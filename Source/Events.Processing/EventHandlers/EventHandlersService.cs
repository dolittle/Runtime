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
            IAsyncStreamReader<Contracts.EventHandlersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<Contracts.EventHandlerRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var hasRegistrationRequest = await HandleRegistrationRequest(runtimeStream, clientStream, context.CancellationToken).ConfigureAwait(false);
            if (!hasRegistrationRequest) return;

            var registration = runtimeStream.Current.RegistrationRequest;
            if (registration.EventHandlerId.To<StreamId>().IsNonWriteable)
            {
                _logger.Warning("Received event handler registration request with Event Handler Id: '{eventHandlerId}' which is an invalid stream id", registration.EventHandlerId.ToGuid());
                await WriteFailedRegistrationResponse(
                    clientStream,
                    new Failure(
                        EventHandlersFailures.CannotRegisterEventHandlerOnNonWriteableStream,
                        $"Received event handler registration request with Event Handler Id: '{registration.EventHandlerId.ToGuid()}' which is an invalid stream id")).ConfigureAwait(false);
                return;
            }

            var dispatcher = _reverseCallDispatchers.GetDispatcherFor(runtimeStream, clientStream, context, _ => _.HandleResult.CallContext, _ => _.HandleRequest.CallContext);
            (var eventProcessor, var filterDefinition) = CreateEventProcessorAndFilterDefinition(registration, dispatcher);

            var registrationResults = await RegisterStreamProcessorsForAllTenants(eventProcessor, filterDefinition, context.CancellationToken).ConfigureAwait(false);
            try
            {
                var allRegistrationsSucceeded = await HandleRegistrationResults(registrationResults, clientStream, context.CancellationToken).ConfigureAwait(false);
                if (!allRegistrationsSucceeded) return;

                await WriteSuccessfulRegistrationResponse(clientStream).ConfigureAwait(false);

                await dispatcher.WaitTillCompleted().ConfigureAwait(false);
            }
            finally
            {
                registrationResults
                    .SelectMany(tenantAndResult => new[] { tenantAndResult.Item2.EventHandlerStreamProcessor, tenantAndResult.Item2.FilterStreamProcessor })
                    .ForEach(_ => _.Dispose());
            }
        }

        async Task<bool> HandleRegistrationResults(
            IEnumerable<(TenantId, EventHandlerRegistrationResult)> registrationResults,
            IServerStreamWriter<Contracts.EventHandlerRuntimeToClientMessage> clientStream,
            CancellationToken cancellationToken)
        {
            var failedRegistrationReasons = registrationResults
                                                .Where(tenantAndResult => tenantAndResult.Item2.Succeeded)
                                                .Select(tenantAndResult => $"For tenant '{tenantAndResult.Item1}':\n\t{string.Join("\n\t", tenantAndResult.Item2.FailureReason.Value.Split("\n"))}");
            if (failedRegistrationReasons.Any())
            {
                var failureMessage = $"Failed to register event handler:\n\t";
                failureMessage += string.Join("\n\t", failedRegistrationReasons);
                _logger.Warning(failureMessage);
                await WriteFailedRegistrationResponse(clientStream, new Failure(EventHandlersFailures.FailedToRegisterEventHandler, failureMessage)).ConfigureAwait(false);
                return false;
            }

            foreach ((var tenant, var result) in registrationResults)
            {
                _executionContextManager.CurrentFor(tenant);
                var filterDefinitions = _getFilterDefinitions();
                await filterDefinitions.PersistFilter(result.FilterProcessor.Definition, cancellationToken).ConfigureAwait(false);
                _ = result.FilterStreamProcessor.Start();
                _ = result.EventHandlerStreamProcessor.Start();
            }

            return true;
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

        (IEventProcessor, TypeFilterWithEventSourcePartitionDefinition) CreateEventProcessorAndFilterDefinition(
            Contracts.EventHandlersRegistrationRequest registration,
            IReverseCallDispatcher<Contracts.EventHandlersClientToRuntimeMessage, Contracts.EventHandlerRuntimeToClientMessage> dispatcher)
        {
            var sourceStream = StreamId.AllStreamId;
            var eventHandlerId = registration.EventHandlerId.To<EventProcessorId>();
            StreamId targetStream = eventHandlerId.Value;
            var scope = registration.ScopeId.To<ScopeId>();
            var types = registration.Types_.Select(_ => _.Id.To<ArtifactId>());
            var partitioned = registration.Partitioned;

            return (
                new EventProcessor(scope, eventHandlerId, dispatcher, _executionContextManager, _logger),
                new TypeFilterWithEventSourcePartitionDefinition(sourceStream, targetStream, types, partitioned));
        }

        async Task<bool> HandleRegistrationRequest(
            IAsyncStreamReader<Contracts.EventHandlersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<Contracts.EventHandlerRuntimeToClientMessage> clientStream,
            CancellationToken cancellationToken)
        {
            if (!await runtimeStream.MoveNext(cancellationToken).ConfigureAwait(false))
            {
                const string message = "EventHandlers connection requested but client-to-runtime stream did not contain any messages";
                _logger.Warning(message);
                await WriteFailedRegistrationResponse(clientStream, new Failure(EventHandlersFailures.NoEventHandlerRegistrationReceived, message)).ConfigureAwait(false);
                return false;
            }

            if (runtimeStream.Current.MessageCase != Contracts.EventHandlersClientToRuntimeMessage.MessageOneofCase.RegistrationRequest)
            {
                const string message = "EventHandlers connection requested but first message in request stream was not an event handler registration request message";
                _logger.Warning(message);
                await WriteFailedRegistrationResponse(clientStream, new Failure(EventHandlersFailures.NoEventHandlerRegistrationReceived, $"The first message in the event handler connection needs to be {typeof(Contracts.EventHandlersRegistrationRequest)}")).ConfigureAwait(false);
                return false;
            }

            return true;
        }

        Task WriteFailedRegistrationResponse(IServerStreamWriter<Contracts.EventHandlerRuntimeToClientMessage> clientStream, Failure failure) =>
            clientStream.WriteAsync(new Contracts.EventHandlerRuntimeToClientMessage { RegistrationResponse = new Contracts.EventHandlerRegistrationResponse { Failure = failure } });

        Task WriteSuccessfulRegistrationResponse(IServerStreamWriter<Contracts.EventHandlerRuntimeToClientMessage> clientStream) =>
            clientStream.WriteAsync(new Contracts.EventHandlerRuntimeToClientMessage { RegistrationResponse = new Contracts.EventHandlerRegistrationResponse() });
    }
}