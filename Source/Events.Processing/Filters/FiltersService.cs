// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Dolittle.Tenancy;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    public class FiltersService : FiltersBase
    {
        readonly ITenants _tenants;
        readonly FactoryFor<IFilters> _getFilters;
        readonly IExecutionContextManager _executionContextManager;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants" />.</param>
        /// <param name="getFilters">The <see cref="FactoryFor{T}" /> <see cref="IFilters" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FiltersService(
            ITenants tenants,
            FactoryFor<IFilters> getFilters,
            IExecutionContextManager executionContextManager,
            IReverseCallDispatchers reverseCallDispatchers,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            ILogger<FiltersService> logger)
        {
            _tenants = tenants;
            _getFilters = getFilters;
            _executionContextManager = executionContextManager;
            _reverseCallDispatchers = reverseCallDispatchers;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<Contracts.FiltersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<Contracts.FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var hasRegistrationRequest = await HandleRegistrationRequest(runtimeStream, clientStream, context.CancellationToken).ConfigureAwait(false);
            if (!hasRegistrationRequest) return;

            var registration = runtimeStream.Current.RegistrationRequest;
            var filterId = registration.FilterId.To<StreamId>();
            if (filterId.IsNonWriteable)
            {
                _logger.Warning("Received filter registration request with Filter Id: '{filterId}' which is an invalid stream id", filterId);
                await WriteFailedRegistrationResponse(clientStream, new Failure(FiltersFailures.CannotRegisterFilterOnNonWriteableStream, $"Received filter registration request with Filter Id: '{filterId}' which is an invalid stream id")).ConfigureAwait(false);
                return;
            }

            var dispatcher = _reverseCallDispatchers.GetDispatcherFor(runtimeStream, clientStream, context, _ => _.FilterResult.CallContext, _ => _.FilterRequest.CallContext);

            var scope = registration.ScopeId.To<ScopeId>();
            var streamId = StreamId.AllStreamId;

            var registrationResults = await RegisterStreamProcessorsForAllTenants(
                scope,
                StreamId.AllStreamId,
                filterId,
                dispatcher,
                context.CancellationToken).ConfigureAwait(false);
            try
            {
                var allRegistrationsSucceeded = await HandleRegistrationResults(registrationResults, clientStream).ConfigureAwait(false);
                if (!allRegistrationsSucceeded) return;

                await WriteSuccessfulRegistrationResponse(clientStream).ConfigureAwait(false);

                await dispatcher.WaitTillCompleted().ConfigureAwait(false);
            }
            finally
            {
                registrationResults
                    .SelectMany(tenantAndResult => new[] { tenantAndResult.Item2.FilterStreamProcessor })
                    .ForEach(_ => _.Dispose());
            }
        }

        async Task<bool> HandleRegistrationResults(
            IEnumerable<(TenantId, FilterRegistrationResult<RemoteFilterDefinition>)> registrationResults,
            IServerStreamWriter<Contracts.FilterRuntimeToClientMessage> clientStream)
        {
            var failedRegistrationReasons = registrationResults
                                                .Where(tenantAndResult => tenantAndResult.Item2.Succeeded)
                                                .Select(tenantAndResult => $"For tenant '{tenantAndResult.Item1}':\n\t{string.Join("\n\t", tenantAndResult.Item2.FailureReason.Value.Split("\n"))}");
            if (failedRegistrationReasons.Any())
            {
                var failureMessage = $"Failed to register filter:\n\t";
                failureMessage += string.Join("\n\t", failedRegistrationReasons);
                _logger.Warning(failureMessage);
                await WriteFailedRegistrationResponse(clientStream, new Failure(FiltersFailures.FailedToRegisterFilter, failureMessage)).ConfigureAwait(false);
                return false;
            }

            foreach ((var tenant, var result) in registrationResults)
            {
                _ = result.FilterStreamProcessor.Start();
            }

            return true;
        }

        async Task<IEnumerable<(TenantId, FilterRegistrationResult<RemoteFilterDefinition>)>> RegisterStreamProcessorsForAllTenants(
            ScopeId scope,
            StreamId sourceStream,
            StreamId targetStream,
            IReverseCallDispatcher<Contracts.FiltersClientToRuntimeMessage, Contracts.FilterRuntimeToClientMessage> dispatcher,
            CancellationToken cancellationToken)
        {
            var registrationResults = new List<(TenantId, FilterRegistrationResult<RemoteFilterDefinition>)>();
            foreach (var tenant in _tenants.All)
            {
                _executionContextManager.CurrentFor(tenant);
                var filterProcessor = new FilterProcessor(
                    scope,
                    new RemoteFilterDefinition(sourceStream, targetStream),
                    dispatcher,
                    _getEventsToStreamsWriter(),
                    _executionContextManager,
                    _logger);
                var filters = _getFilters();
                var registrationResult = await filters.Register(filterProcessor, cancellationToken).ConfigureAwait(false);
                registrationResults.Add((tenant, registrationResult));
            }

            return registrationResults.AsEnumerable();
        }

        async Task<bool> HandleRegistrationRequest(
            IAsyncStreamReader<Contracts.FiltersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<Contracts.FilterRuntimeToClientMessage> clientStream,
            CancellationToken cancellationToken)
        {
            if (!await runtimeStream.MoveNext(cancellationToken).ConfigureAwait(false))
            {
                const string message = "Filters connection requested but client-to-runtime stream did not contain any messages";
                _logger.Warning(message);
                await WriteFailedRegistrationResponse(clientStream, new Failure(FiltersFailures.NoFilterRegistrationReceived, message)).ConfigureAwait(false);
                return false;
            }

            if (runtimeStream.Current.MessageCase != Contracts.FiltersClientToRuntimeMessage.MessageOneofCase.RegistrationRequest)
            {
                const string message = "Filters connection requested but first message in request stream was not a filter registration request message";
                _logger.Warning(message);
                await WriteFailedRegistrationResponse(clientStream, new Failure(FiltersFailures.NoFilterRegistrationReceived, $"The first message in the filter connection needs to be {typeof(Contracts.FiltersRegistrationRequest)}")).ConfigureAwait(false);
                return false;
            }

            return true;
        }

        Task WriteFailedRegistrationResponse(IServerStreamWriter<Contracts.FilterRuntimeToClientMessage> clientStream, Failure failure) =>
            clientStream.WriteAsync(new Contracts.FilterRuntimeToClientMessage { RegistrationResponse = new Contracts.FilterRegistrationResponse { Failure = failure } });

        Task WriteSuccessfulRegistrationResponse(IServerStreamWriter<Contracts.FilterRuntimeToClientMessage> clientStream) =>
            clientStream.WriteAsync(new Contracts.FilterRuntimeToClientMessage { RegistrationResponse = new Contracts.FilterRegistrationResponse() });
    }
}