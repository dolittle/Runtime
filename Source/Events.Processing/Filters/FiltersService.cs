// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
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
        readonly IStreamProcessors _streamProcessors;
        readonly IValidateFilterForAllTenants _filterForAllTenants;
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly IExecutionContextManager _executionContextManager;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly FactoryFor<IWriteEventsToPublicStreams> _getEventsToPublicStreamsWriter;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitionRepository;
        readonly ILoggerManager _loggerManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="filterForAllTenants">The <see cref="IValidateFilterForAllTenants" />.</param>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="getEventsToPublicStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToPublicStreams" />.</param>
        /// <param name="getStreamDefinitionRepository">The <see cref="FactoryFor{T}" /> <see cref="IFilterDefinitions" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager"/>.</param>
        public FiltersService(
            IStreamProcessors streamProcessors,
            IValidateFilterForAllTenants filterForAllTenants,
            IPerformActionOnAllTenants onAllTenants,
            IExecutionContextManager executionContextManager,
            IReverseCallDispatchers reverseCallDispatchers,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            FactoryFor<IWriteEventsToPublicStreams> getEventsToPublicStreamsWriter,
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitionRepository,
            ILoggerManager loggerManager)
        {
            _streamProcessors = streamProcessors;
            _filterForAllTenants = filterForAllTenants;
            _onAllTenants = onAllTenants;
            _executionContextManager = executionContextManager;
            _reverseCallDispatchers = reverseCallDispatchers;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _getEventsToPublicStreamsWriter = getEventsToPublicStreamsWriter;
            _getStreamDefinitionRepository = getStreamDefinitionRepository;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<FiltersService>();
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
            var sourceStreamId = StreamId.EventLog;
            if (await RejectIfInvalidFilterId(dispatcher, filterId, context.CancellationToken).ConfigureAwait(false)) return;

            var filterDefinition = new FilterDefinition(sourceStreamId, filterId, partitioned: false);
            await RegisterFilter(
                dispatcher,
                scopeId,
                filterDefinition,
                () => new FilterProcessor(
                    scopeId,
                    filterDefinition,
                    dispatcher,
                    _getEventsToStreamsWriter(),
                    _loggerManager.CreateLogger<FilterProcessor>()),
                context.CancellationToken).ConfigureAwait(false);
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
            var sourceStreamId = StreamId.EventLog;
            if (await RejectIfInvalidFilterId(dispatcher, filterId, context.CancellationToken).ConfigureAwait(false)) return;

            var filterDefinition = new FilterDefinition(sourceStreamId, filterId, partitioned: true);

            await RegisterFilter(
                dispatcher,
                scopeId,
                filterDefinition,
                () => new Partitioned.FilterProcessor(
                    scopeId,
                    filterDefinition,
                    dispatcher,
                    _getEventsToStreamsWriter(),
                    _loggerManager.CreateLogger<Partitioned.FilterProcessor>()),
                context.CancellationToken).ConfigureAwait(false);
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
            var sourceStreamId = StreamId.EventLog;

            if (await RejectIfInvalidFilterId(dispatcher, filterId, context.CancellationToken).ConfigureAwait(false)) return;

            var filterDefinition = new PublicFilterDefinition(sourceStreamId, filterId);
            await RegisterFilter(
                dispatcher,
                scopeId,
                filterDefinition,
                () => new PublicFilterProcessor(
                    filterDefinition,
                    dispatcher,
                    _getEventsToPublicStreamsWriter(),
                    _loggerManager.CreateLogger<PublicFilterProcessor>()),
                context.CancellationToken).ConfigureAwait(false);
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
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
        {
            var streamDefinition = new StreamDefinition(filterDefinition);
            var successfullyRegisteredFilter = _streamProcessors.TryRegister(
                scopeId,
                filterDefinition.TargetStream.Value,
                streamDefinition,
                () => getFilterProcessor(),
                cancellationToken,
                out var filterStreamProcessor);
            if (!successfullyRegisteredFilter)
            {
                _logger.Warning("Failed to register Filter: {filterId}. Filter already registered", filterDefinition.TargetStream);
                var failure = new Failure(
                    FiltersFailures.FailedToRegisterFilter,
                    $"Failed to register Filter: {filterDefinition.TargetStream}. Filter already registered.");
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                return;
            }

            await filterStreamProcessor.Initialize().ConfigureAwait(false);
            Task runningDispatcher;
            try
            {
                runningDispatcher = dispatcher.Accept(new FilterRegistrationResponse(), cancellationToken);
                var filterValidationResults = await _filterForAllTenants.Validate(getFilterProcessor, cancellationToken).ConfigureAwait(false);

                if (filterValidationResults.Any(_ => !_.Value.Succeeded))
                {
                    var firstFailedValidation = filterValidationResults.Select(_ => _.Value).First(_ => !_.Succeeded);
                    _logger.Warning("Failed to register Filter: {filterId}. Filter validation failed. {reason}", filterDefinition.TargetStream, firstFailedValidation.FailureReason);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Filter: {filterDefinition.TargetStream}. Filter validation failed. {firstFailedValidation.FailureReason}");
                    await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                }

                await _onAllTenants.PerformAsync(_ => _getStreamDefinitionRepository().Persist(scopeId, streamDefinition, cancellationToken)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error while registering Filter: {filterId}", filterDefinition.TargetStream);
                filterStreamProcessor.Unregister();
                throw;
            }

            try
            {
                await Task.WhenAny(filterStreamProcessor.Start(), runningDispatcher).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.Debug(ex, "Filter: '{filterId}' failed", filterDefinition.TargetStream);
                }
            }
            finally
            {
                _logger.Debug("Filter: '{filterId}' stopped", filterDefinition.TargetStream);
            }
        }

        Task WriteFailedRegistrationResponse<TClientMessage, TConnectRequest, TResponse>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            Failure failure,
            CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class => dispatcher.Reject(new FilterRegistrationResponse { Failure = failure }, cancellationToken);
    }
}