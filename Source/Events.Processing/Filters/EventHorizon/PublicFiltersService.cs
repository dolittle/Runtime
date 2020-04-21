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
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Dolittle.Tenancy;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.PublicFilters;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon
{
    /// <summary>
    /// Represents the implementation of <see creF="PublicFiltersBase"/>.
    /// </summary>
    public class PublicFiltersService : PublicFiltersBase
    {
        readonly ITenants _tenants;
        readonly FactoryFor<IFilters> _getFilters;
        readonly IExecutionContextManager _executionContextManager;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly FactoryFor<IWriteEventsToPublicStreams> _getEventsToPublicStreamsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFiltersService"/> class.
        /// </summary>
        /// <param name="tenants">The <see cref="ITenants" />.</param>
        /// <param name="getFilters"><see cref="FactoryFor{T}" /> <see cref="IFilters" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/>.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="getEventsToPublicStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToPublicStreams" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PublicFiltersService(
            ITenants tenants,
            FactoryFor<IFilters> getFilters,
            IExecutionContextManager executionContextManager,
            IReverseCallDispatchers reverseCallDispatchers,
            FactoryFor<IWriteEventsToPublicStreams> getEventsToPublicStreamsWriter,
            ILogger<PublicFiltersService> logger)
        {
            _tenants = tenants;
            _getFilters = getFilters;
            _executionContextManager = executionContextManager;
            _reverseCallDispatchers = reverseCallDispatchers;
            _getEventsToPublicStreamsWriter = getEventsToPublicStreamsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<PublicFiltersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var dispatcher = _reverseCallDispatchers.GetDispatcherFor<PublicFiltersClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFiltersRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse>(
                runtimeStream,
                clientStream,
                context,
                _ => _.RegistrationRequest,
                (serverMessage, registrationResponse) => serverMessage.RegistrationResponse = registrationResponse,
                (serverMessage, request) => serverMessage.FilterRequest = request,
                _ => _.FilterResult,
                _ => _.CallContext,
                (request, context) => request.CallContext = context,
                _ => _.CallContext);

            if (!await dispatcher.ReceiveArguments(context.CancellationToken).ConfigureAwait(false))
            {
                const string message = "Public Filters connection arguments were not received";
                _logger.Warning(message);
                var failure = new Failure(FiltersFailures.NoFilterRegistrationReceived, message);
                await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                return;
            }

            var arguments = dispatcher.Arguments;
            var filterId = arguments.FilterId.To<StreamId>();
            if (filterId.IsNonWriteable)
            {
                _logger.Warning("Received public filter registration request with Filter Id: '{filterId}' which is an invalid stream id", filterId);
                var failure = new Failure(FiltersFailures.CannotRegisterFilterOnNonWriteableStream, $"Received public filter registration request with Filter Id: '{filterId}' which is an invalid stream id");
                await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                return;
            }

            var streamId = StreamId.AllStreamId;

            var registrationResults = await RegisterStreamProcessorsForAllTenants(
                StreamId.AllStreamId,
                filterId,
                dispatcher,
                context.CancellationToken).ConfigureAwait(false);
            try
            {
                if (!TryStartStreamProcessors(registrationResults, out var failure))
                {
                    _logger.Warning(failure.Reason);
                    await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                    return;
                }

                await dispatcher.Accept(new FilterRegistrationResponse(), context.CancellationToken).ConfigureAwait(false);
            }
            finally
            {
                registrationResults
                    .SelectMany(tenantAndResult => new[] { tenantAndResult.Item2.FilterStreamProcessor })
                    .ForEach(_ => _.Dispose());
            }
        }

        bool TryStartStreamProcessors(IEnumerable<(TenantId, FilterRegistrationResult<PublicFilterDefinition>)> registrationResults, out Failure failure)
        {
            failure = null;
            var failedRegistrationReasons = registrationResults
                                                .Where(tenantAndResult => !tenantAndResult.Item2.Succeeded)
                                                .Select(tenantAndResult => $"For tenant '{tenantAndResult.Item1}':\n\t{string.Join("\n\t", tenantAndResult.Item2.FailureReason.Value.Split("\n"))}");
            if (failedRegistrationReasons.Any())
            {
                var failureMessage = $"Failed to register public filter:\n\t";
                failureMessage += string.Join("\n\t", failedRegistrationReasons);
                failure = new Failure(FiltersFailures.FailedToRegisterFilter, failureMessage);
                return false;
            }

            foreach ((var tenant, var result) in registrationResults)
            {
                _ = result.FilterStreamProcessor.Start();
            }

            return true;
        }

        async Task<IEnumerable<(TenantId, FilterRegistrationResult<PublicFilterDefinition>)>> RegisterStreamProcessorsForAllTenants(
            StreamId sourceStream,
            StreamId targetStream,
            IReverseCallDispatcher<PublicFiltersClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFiltersRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse> dispatcher,
            CancellationToken cancellationToken)
        {
            var registrationResults = new List<(TenantId, FilterRegistrationResult<PublicFilterDefinition>)>();
            foreach (var tenant in _tenants.All)
            {
                _executionContextManager.CurrentFor(tenant);
                var filterProcessor = new PublicFilterProcessor(
                    new PublicFilterDefinition(sourceStream, targetStream),
                    dispatcher,
                    _getEventsToPublicStreamsWriter(),
                    _logger);
                var filters = _getFilters();
                var registrationResult = await filters.Register(filterProcessor, cancellationToken).ConfigureAwait(false);
                registrationResults.Add((tenant, registrationResult));
            }

            return registrationResults.AsEnumerable();
        }

        Task WriteFailedRegistrationResponse(
            IReverseCallDispatcher<PublicFiltersClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFiltersRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse> dispatcher,
            Failure failure,
            CancellationToken cancellationToken) => dispatcher.Reject(new FilterRegistrationResponse { Failure = failure }, cancellationToken);
    }
}