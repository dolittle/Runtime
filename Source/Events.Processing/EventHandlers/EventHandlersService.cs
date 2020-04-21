// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Dolittle.Tenancy;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.EventHandlers;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents the implementation of <see cref="EventHandlersBase"/>.
    /// </summary>
    public class EventHandlersService : EventHandlersBase
    {
        readonly ITenants _tenants;
        readonly FactoryFor<IEventHandlers> _getEventHandlers;
        readonly FactoryFor<IFilterDefinitionRepository> _getFilterDefinitions;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants" />.</param>
        /// <param name="getEventHandlers">The <see cref="FactoryFor{T}" /> <see cref="IEventHandlers" />.</param>
        /// <param name="getFilterDefinitions">The <see cref="FactoryFor{T}" /> <see cref="IFilterDefinitionRepository" />.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHandlersService(
            ITenants tenants,
            FactoryFor<IEventHandlers> getEventHandlers,
            FactoryFor<IFilterDefinitionRepository> getFilterDefinitions,
            IReverseCallDispatchers reverseCallDispatchers,
            IExecutionContextManager executionContextManager,
            ILogger<EventHandlersService> logger)
        {
            _tenants = tenants;
            _getEventHandlers = getEventHandlers;
            _getFilterDefinitions = getFilterDefinitions;
            _reverseCallDispatchers = reverseCallDispatchers;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<EventHandlersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<EventHandlerRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var dispatcher = _reverseCallDispatchers.GetDispatcherFor<EventHandlersClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlersRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>(
                runtimeStream,
                clientStream,
                context,
                _ => _.RegistrationRequest,
                (serverMessage, registrationResponse) => serverMessage.RegistrationResponse = registrationResponse,
                (serverMessage, request) => serverMessage.HandleRequest = request,
                _ => _.HandleResult,
                _ => _.CallContext,
                (request, context) => request.CallContext = context,
                _ => _.CallContext);
            if (!await dispatcher.ReceiveArguments(context.CancellationToken).ConfigureAwait(false))
            {
                const string message = "Event Handlers connection arguments were not received";
                _logger.Warning(message);
                var failure = new Failure(EventHandlersFailures.NoEventHandlerRegistrationReceived, message);
                await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                return;
            }

            var arguments = dispatcher.Arguments;
            if (arguments.EventHandlerId.To<StreamId>().IsNonWriteable)
            {
                _logger.Warning("Received event handler registration request with Event Handler Id: '{eventHandlerId}' which is an invalid stream id", arguments.EventHandlerId.ToGuid());
                var failure = new Failure(
                    EventHandlersFailures.CannotRegisterEventHandlerOnNonWriteableStream,
                    $"Received event handler registration request with Event Handler Id: '{arguments.EventHandlerId.ToGuid()}' which is an invalid stream id");
                await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                return;
            }

            var sourceStream = StreamId.AllStreamId;
            var eventHandlerId = arguments.EventHandlerId.To<EventProcessorId>();
            StreamId targetStream = eventHandlerId.Value;
            var scope = arguments.ScopeId.To<ScopeId>();
            var types = arguments.Types_.Select(_ => _.Id.To<ArtifactId>());
            var partitioned = arguments.Partitioned;

            var eventProcessor = new EventProcessor(scope, eventHandlerId, dispatcher, _logger);
            var filterDefinition = new TypeFilterWithEventSourcePartitionDefinition(sourceStream, targetStream, types, partitioned);

            var registrationResults = await RegisterStreamProcessorsForAllTenants(eventProcessor, filterDefinition, context.CancellationToken).ConfigureAwait(false);
            try
            {
                if (TryStartStreamProcessors(registrationResults, out var failure))
                {
                    _logger.Warning(failure.Reason);
                    await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                    return;
                }

                await PersistFilters(registrationResults.Select(_ => (_.Item1, _.Item2.FilterProcessor.Definition as IPersistableFilterDefinition)), context.CancellationToken).ConfigureAwait(false);
                await dispatcher.Accept(new EventHandlerRegistrationResponse(), context.CancellationToken).ConfigureAwait(false);
            }
            finally
            {
                registrationResults
                    .SelectMany(tenantAndResult => new[] { tenantAndResult.Item2.EventHandlerStreamProcessor, tenantAndResult.Item2.FilterStreamProcessor })
                    .ForEach(_ => _.Dispose());
            }
        }

        bool TryStartStreamProcessors(IEnumerable<(TenantId, EventHandlerRegistrationResult)> registrationResults, out Failure failure)
        {
            failure = null;
            var failedRegistrationReasons = registrationResults
                                                .Where(tenantAndResult => !tenantAndResult.Item2.Succeeded)
                                                .Select(tenantAndResult => $"For tenant '{tenantAndResult.Item1}':\n\t{string.Join("\n\t", tenantAndResult.Item2.FailureReason.Value.Split("\n"))}");
            if (failedRegistrationReasons.Any())
            {
                var failureMessage = $"Failed to register event handler:\n\t";
                failureMessage += string.Join("\n\t", failedRegistrationReasons);
                failure = new Failure(EventHandlersFailures.FailedToRegisterEventHandler, failureMessage);
                return false;
            }

            foreach ((var tenant, var result) in registrationResults)
            {
                _ = result.FilterStreamProcessor.Start();
                _ = result.EventHandlerStreamProcessor.Start();
            }

            return true;
        }

        async Task PersistFilters(IEnumerable<(TenantId, IPersistableFilterDefinition filter)> tenantsAndFilters, CancellationToken cancellationToken)
        {
            foreach ((var tenant, var filter) in tenantsAndFilters)
            {
                _executionContextManager.CurrentFor(tenant);
                var filterDefinitions = _getFilterDefinitions();
                await filterDefinitions.PersistFilter(filter, cancellationToken).ConfigureAwait(false);
            }
        }

        async Task<IEnumerable<(TenantId, EventHandlerRegistrationResult)>> RegisterStreamProcessorsForAllTenants(
            IEventProcessor eventProcessor,
            TypeFilterWithEventSourcePartitionDefinition filterDefinition,
            CancellationToken cancellationToken)
        {
            var registrationResults = new List<(TenantId, EventHandlerRegistrationResult)>();
            foreach (var tenant in _tenants.All)
            {
                _executionContextManager.CurrentFor(tenant);
                var eventHandlers = _getEventHandlers();
                var registrationResult = await eventHandlers.Register(filterDefinition, eventProcessor, cancellationToken).ConfigureAwait(false);
                registrationResults.Add((tenant, registrationResult));
            }

            return registrationResults.AsEnumerable();
        }

        Task WriteFailedRegistrationResponse(
            IReverseCallDispatcher<EventHandlersClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlersRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse> dispatcher,
            Failure failure,
            CancellationToken cancellationToken) => dispatcher.Reject(new EventHandlerRegistrationResponse { Failure = failure }, cancellationToken);
    }
}