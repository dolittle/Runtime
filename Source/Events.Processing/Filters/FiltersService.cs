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
using Dolittle.Runtime.Events.Processing.Filters.EventHorizon;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Google.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    public class FiltersService : FiltersBase
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly IRegisterStreamProcessorForAllTenants _streamProcessorForAllTenants;
        readonly IFilterValidators _filterValidators;
        readonly IExecutionContextManager _executionContextManager;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly FactoryFor<IWriteEventsToPublicStreams> _getEventsToPublicStreamsWriter;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitionRepository;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="streamProcessorForAllTenants">The <see cref="IRegisterStreamProcessorForAllTenants" />.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="getEventsToPublicStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToPublicStreams" />.</param>
        /// <param name="getStreamDefinitionRepository">The <see cref="FactoryFor{T}" /> <see cref="IFilterDefinitionRepository" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FiltersService(
            IPerformActionOnAllTenants onAllTenants,
            IRegisterStreamProcessorForAllTenants streamProcessorForAllTenants,
            IFilterValidators filterValidators,
            IExecutionContextManager executionContextManager,
            IReverseCallDispatchers reverseCallDispatchers,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            FactoryFor<IWriteEventsToPublicStreams> getEventsToPublicStreamsWriter,
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitionRepository,
            ILogger<FiltersService> logger)
        {
            _onAllTenants = onAllTenants;
            _streamProcessorForAllTenants = streamProcessorForAllTenants;
            _filterValidators = filterValidators;
            _executionContextManager = executionContextManager;
            _reverseCallDispatchers = reverseCallDispatchers;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _getEventsToPublicStreamsWriter = getEventsToPublicStreamsWriter;
            _getStreamDefinitionRepository = getStreamDefinitionRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<FiltersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var dispatcher = _reverseCallDispatchers.GetFor<FiltersClientToRuntimeMessage, FilterRuntimeToClientMessage, FiltersRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse>(
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

            if (await RejectIfNotReceivedArguments(dispatcher, context.CancellationToken).ConfigureAwait(false)) return;

            var arguments = dispatcher.Arguments;
            _executionContextManager.CurrentFor(arguments.CallContext.ExecutionContext);

            var filterId = arguments.FilterId.To<StreamId>();
            var scopeId = arguments.ScopeId.To<ScopeId>();
            var sourceStreamId = StreamId.AllStreamId;
            if (await RejectIfInvalidFilterId(dispatcher, filterId, context.CancellationToken).ConfigureAwait(false)) return;

            var filterDefinition = new RemoteFilterDefinition(sourceStreamId, filterId, partitioned: false);
            using var filterRegistration = CreateFilterRegistration(
                scopeId,
                filterDefinition,
                () => Task.FromResult<IFilterProcessor<RemoteFilterDefinition>>(
                    new FilterProcessor(
                        scopeId,
                        filterDefinition,
                        dispatcher,
                        _getEventsToStreamsWriter(),
                        _logger)),
                context.CancellationToken);
            await RegisterFilter(dispatcher, filterId, filterRegistration, context.CancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task ConnectPartitioned(
            IAsyncStreamReader<PartitionedFiltersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var dispatcher = _reverseCallDispatchers.GetFor<PartitionedFiltersClientToRuntimeMessage, FilterRuntimeToClientMessage, PartitionedFiltersRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse>(
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

            if (await RejectIfNotReceivedArguments(dispatcher, context.CancellationToken).ConfigureAwait(false)) return;

            var arguments = dispatcher.Arguments;
            _executionContextManager.CurrentFor(arguments.CallContext.ExecutionContext);

            var filterId = arguments.FilterId.To<StreamId>();
            var scopeId = arguments.ScopeId.To<ScopeId>();
            var sourceStreamId = StreamId.AllStreamId;
            if (await RejectIfInvalidFilterId(dispatcher, filterId, context.CancellationToken).ConfigureAwait(false)) return;

            var filterDefinition = new RemoteFilterDefinition(sourceStreamId, filterId, partitioned: true);

            using var filterRegistration = CreateFilterRegistration(
                scopeId,
                filterDefinition,
                () => Task.FromResult<IFilterProcessor<RemoteFilterDefinition>>(
                    new Partitioned.FilterProcessor(
                        scopeId,
                        filterDefinition,
                        dispatcher,
                        _getEventsToStreamsWriter(),
                        _logger)),
                context.CancellationToken);
            await RegisterFilter(dispatcher, filterId, filterRegistration, context.CancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task ConnectPublic(
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

            if (await RejectIfNotReceivedArguments(dispatcher, context.CancellationToken).ConfigureAwait(false)) return;

            var arguments = dispatcher.Arguments;
            _executionContextManager.CurrentFor(arguments.CallContext.ExecutionContext);

            var filterId = arguments.FilterId.To<StreamId>();
            var scopeId = ScopeId.Default;
            var sourceStreamId = StreamId.AllStreamId;

            if (await RejectIfInvalidFilterId(dispatcher, filterId, context.CancellationToken).ConfigureAwait(false)) return;

            var filterDefinition = new PublicFilterDefinition(sourceStreamId, filterId);
            using var filterRegistration = CreateFilterRegistration(
                scopeId,
                filterDefinition,
                () => Task.FromResult<IFilterProcessor<PublicFilterDefinition>>(
                    new PublicFilterProcessor(
                        filterDefinition,
                        dispatcher,
                        _getEventsToPublicStreamsWriter(),
                        _logger)),
                context.CancellationToken);
            await RegisterFilter(dispatcher, filterId, filterRegistration, context.CancellationToken).ConfigureAwait(false);
        }

        async Task<bool> RejectIfNotReceivedArguments<TClientMessage, TConnectRequest, TResponse>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
        {
            if (!await dispatcher.ReceiveArguments(cancellationToken).ConfigureAwait(false))
            {
                const string message = "Connection arguments were not received";
                _logger.Warning(message);
                var failure = new Failure(FiltersFailures.NoFilterRegistrationReceived, message);
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                return true;
            }

            return false;
        }

        async Task<bool> RejectIfInvalidFilterId<TClientMessage, TConnectRequest, TResponse>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            StreamId filterId,
            CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
        {
            if (filterId.IsNonWriteable)
            {
                _logger.Warning("Filter Id: '{filterId}' is an invalid Stream Id", filterId);
                var failure = new Failure(FiltersFailures.CannotRegisterFilterOnNonWriteableStream, $"Filter Id: '{filterId}' is an invalid Stream Id");
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                return true;
            }

            return false;
        }

        async Task RegisterFilter<TFilterDefinition, TClientMessage, TConnectRequest, TResponse>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            StreamId filterId,
            FilterRegistration<TFilterDefinition> filterRegistration,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
        {
            try
            {
                var registrationResult = await filterRegistration.Register().ConfigureAwait(false);
                if (!registrationResult.Succeeded)
                {
                    _logger.Warning("Failed during registration of Filter: '{filterId}'. {reason}", filterId, registrationResult.FailureReason);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed during registration of Filter: '{filterId}'. {registrationResult.FailureReason}");

                    await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await filterRegistration.Complete().ConfigureAwait(false);
                    await dispatcher.Accept(new FilterRegistrationResponse(), cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.Debug(ex, "Filter: '{filterId}' failed", filterId);
                }

                if (!filterRegistration.Completed) await filterRegistration.Fail().ConfigureAwait(false);
            }
            finally
            {
                _logger.Debug("Filter: '{filterId}' stopped", filterId);
            }
        }

        FilterRegistration<TFilterDefinition> CreateFilterRegistration<TFilterDefinition>(
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<Task<IFilterProcessor<TFilterDefinition>>> getFilterProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition =>
                new FilterRegistration<TFilterDefinition>(
                    scopeId,
                    filterDefinition,
                    getFilterProcessor,
                    _onAllTenants,
                    _streamProcessorForAllTenants,
                    _filterValidators,
                    _getStreamDefinitionRepository,
                    cancellationToken);

        Task WriteFailedRegistrationResponse<TClientMessage, TConnectRequest, TResponse>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            Failure failure,
            CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class => dispatcher.Reject(new FilterRegistrationResponse { Failure = failure }, cancellationToken);
    }
}