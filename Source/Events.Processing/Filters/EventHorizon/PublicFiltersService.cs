// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.PublicFilters;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon
{
    /// <summary>
    /// Represents the implementation of <see creF="PublicFiltersBase"/>.
    /// </summary>
    public class PublicFiltersService : PublicFiltersBase
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly IRegisterStreamProcessorForAllTenants _streamProcessorForAllTenants;
        readonly IFilterValidators _filterValidators;
        readonly IExecutionContextManager _executionContextManager;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly FactoryFor<IWriteEventsToPublicStreams> _getEventsToStreamsWriter;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitionRepository;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFiltersService"/> class.
        /// </summary>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToPublicStreams" />.</param>
        /// <param name="getStreamDefinitionRepository">The <see cref="FactoryFor{T}" /> <see cref="IFilterDefinitionRepository" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PublicFiltersService(
            IPerformActionOnAllTenants onAllTenants,
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            IFilterValidators filterValidators,
            IExecutionContextManager executionContextManager,
            IReverseCallDispatchers reverseCallDispatchers,
            FactoryFor<IWriteEventsToPublicStreams> getEventsToStreamsWriter,
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitionRepository,
            ILogger<PublicFiltersService> logger)
        {
            _onAllTenants = onAllTenants;
            _streamProcessorForAllTenants = streamProcessorForAllTenants;
            _filterValidators = filterValidators;
            _executionContextManager = executionContextManager;
            _reverseCallDispatchers = reverseCallDispatchers;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _getStreamDefinitionRepository = getStreamDefinitionRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<PublicFiltersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var dispatcher = _reverseCallDispatchers.GetFor<PublicFiltersClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFiltersRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse>(
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
            _executionContextManager.CurrentFor(arguments.CallContext.ExecutionContext);

            var filterId = arguments.FilterId.To<StreamId>();
            var scopeId = ScopeId.Default;
            var sourceStreamId = StreamId.AllStreamId;
            if (filterId.IsNonWriteable)
            {
                _logger.Warning("Cannot register Public Filter: '{filterId}' because it is an invalid stream id", filterId);
                var failure = new Failure(FiltersFailures.CannotRegisterFilterOnNonWriteableStream, $"Cannot register Public Filter: '{filterId}' because it is an invalid stream id");
                await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                return;
            }

            var filterDefinition = new PublicFilterDefinition(sourceStreamId, filterId);
            using var filterRegistration = new FilterRegistration<PublicFilterDefinition>(
                scopeId,
                filterDefinition,
                () => Task.FromResult<IFilterProcessor<PublicFilterDefinition>>(new PublicFilterProcessor(
                                        filterDefinition,
                                        dispatcher,
                                        _getEventsToStreamsWriter(),
                                        _logger)),
                _onAllTenants,
                _streamProcessorForAllTenants,
                _filterValidators,
                _getStreamDefinitionRepository,
                context.CancellationToken);
            try
            {
                var registrationResult = await filterRegistration.Register().ConfigureAwait(false);
                if (!registrationResult.Succeeded)
                {
                    _logger.Warning("Failed during registration of Public Filter: '{filterId}'. {reason}", filterId, registrationResult.FailureReason);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed during registration of Public Filter: '{filterId}'. {registrationResult.FailureReason}");

                    await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await filterRegistration.Complete().ConfigureAwait(false);
                    await dispatcher.Accept(new FilterRegistrationResponse(), context.CancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    _logger.Debug(ex, "Public Filter: '{filterId}' failed", filterId);
                }

                if (!filterRegistration.Completed) await filterRegistration.Fail().ConfigureAwait(false);
            }
            finally
            {
                _logger.Debug("Public Filter: '{filterId}' stopped", filterId);
            }
        }

        Task WriteFailedRegistrationResponse(
            IReverseCallDispatcher<PublicFiltersClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFiltersRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse> dispatcher,
            Failure failure,
            CancellationToken cancellationToken) => dispatcher.Reject(new FilterRegistrationResponse { Failure = failure }, cancellationToken);
    }
}