// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Async;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters.EventHorizon;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
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
        readonly IExecutionContextManager _executionContextManager;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly FactoryFor<IWriteEventsToPublicStreams> _getEventsToPublicStreamsWriter;
        readonly IStreamDefinitions _streamDefinitions;
        readonly ILoggerManager _loggerManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="filterForAllTenants">The <see cref="IValidateFilterForAllTenants" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="streamDefinitions">The <see cref="IFilterDefinitions" />.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="getEventsToPublicStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToPublicStreams" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager"/>.</param>
        public FiltersService(
            IStreamProcessors streamProcessors,
            IValidateFilterForAllTenants filterForAllTenants,
            IExecutionContextManager executionContextManager,
            IReverseCallDispatchers reverseCallDispatchers,
            IStreamDefinitions streamDefinitions,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            FactoryFor<IWriteEventsToPublicStreams> getEventsToPublicStreamsWriter,
            ILoggerManager loggerManager)
        {
            _streamProcessors = streamProcessors;
            _filterForAllTenants = filterForAllTenants;
            _executionContextManager = executionContextManager;
            _reverseCallDispatchers = reverseCallDispatchers;
            _streamDefinitions = streamDefinitions;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _getEventsToPublicStreamsWriter = getEventsToPublicStreamsWriter;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<FiltersService>();
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<FilterClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var dispatcher = _reverseCallDispatchers.GetFor<FilterClientToRuntimeMessage, FilterRuntimeToClientMessage, FilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, FilterResponse>(
                runtimeStream,
                clientStream,
                context,
                _ => _.RegistrationRequest,
                (serverMessage, registrationResponse) => serverMessage.RegistrationResponse = registrationResponse,
                (serverMessage, request) => serverMessage.FilterRequest = request,
                _ => _.FilterResult,
                _ => _.CallContext,
                (request, context) => request.CallContext = context,
                _ => _.CallContext,
                (message, ping) => message.Ping = ping,
                message => message.Pong);

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
            IAsyncStreamReader<PartitionedFilterClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var dispatcher = _reverseCallDispatchers.GetFor<PartitionedFilterClientToRuntimeMessage, FilterRuntimeToClientMessage, PartitionedFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse>(
                runtimeStream,
                clientStream,
                context,
                _ => _.RegistrationRequest,
                (serverMessage, registrationResponse) => serverMessage.RegistrationResponse = registrationResponse,
                (serverMessage, request) => serverMessage.FilterRequest = request,
                _ => _.FilterResult,
                _ => _.CallContext,
                (request, context) => request.CallContext = context,
                _ => _.CallContext,
                (message, ping) => message.Ping = ping,
                message => message.Pong);

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
            IAsyncStreamReader<PublicFilterClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var dispatcher = _reverseCallDispatchers.GetFor<PublicFilterClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse>(
                runtimeStream,
                clientStream,
                context,
                _ => _.RegistrationRequest,
                (serverMessage, registrationResponse) => serverMessage.RegistrationResponse = registrationResponse,
                (serverMessage, request) => serverMessage.FilterRequest = request,
                _ => _.FilterResult,
                _ => _.CallContext,
                (request, context) => request.CallContext = context,
                _ => _.CallContext,
                (message, ping) => message.Ping = ping,
                message => message.Pong);

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
            CancellationToken externalCancellationToken)
            where TFilterDefinition : IFilterDefinition
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
        {
            using var internalCancellationTokenSource = new CancellationTokenSource();
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(internalCancellationTokenSource.Token, externalCancellationToken);
            var cancellationToken = linkedTokenSource.Token;

            var tryRegisterFilter = TryRegisterStreamProcessor(scopeId, filterDefinition, getFilterProcessor, cancellationToken);
            if (!tryRegisterFilter.Success)
            {
                linkedTokenSource.Cancel();
                if (tryRegisterFilter.HasException)
                {
                    var exception = tryRegisterFilter.Exception;
                    _logger.Warning(exception, "An error occurred while registering Filter: {filterId}", filterDefinition.TargetStream);
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                }
                else
                {
                    _logger.Debug("Failed to register Filter: {filterId}. Filter already registered", filterDefinition.TargetStream);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Filter: {filterDefinition.TargetStream}. Filter already registered.");
                    await WriteFailedRegistrationResponse(dispatcher, failure, externalCancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            using var filterStreamProcessor = tryRegisterFilter.Result;
            var tryStartFilter = await TryStartFilter(
                dispatcher,
                filterStreamProcessor,
                scopeId,
                filterDefinition,
                getFilterProcessor,
                cancellationToken).ConfigureAwait(false);
            if (!tryStartFilter.Success)
            {
                internalCancellationTokenSource.Cancel();
                if (tryStartFilter.HasException)
                {
                    var exception = tryStartFilter.Exception;
                    _logger.Debug(exception, "An error occurred while starting Filter: '{filterId}' in Scope: {scopeId}", filterDefinition.TargetStream, scopeId);
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                }
                else
                {
                    _logger.Debug("Could not start Filter: '{filterId}' in Scope: {scopeId}", filterDefinition.TargetStream, scopeId);
                    return;
                }
            }

            var tasks = tryStartFilter.Result;
            var anyTask = await Task.WhenAny(tasks).ConfigureAwait(false);
            if (TryGetException(tasks, out var ex))
            {
                internalCancellationTokenSource.Cancel();
                _logger.Warning(ex, "An error occurred while processing Filter: '{filterId}' in Scope: '{scopeId}'", filterDefinition.TargetStream, scopeId);
                await Task.WhenAll(tasks).ConfigureAwait(false);
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            if (!externalCancellationToken.IsCancellationRequested)
            {
                _logger.Warning(ex, "Filter: '{filterId}' in Scope: '{scopeId}' failed", filterDefinition.TargetStream, scopeId);
            }

            _logger.Debug("Filter: '{filterId}' in Scope: '{scopeId}' stopped", filterDefinition.TargetStream, scopeId);
        }

        async Task<Try<IEnumerable<Task>>> TryStartFilter<TClientMessage, TConnectRequest, TResponse, TFilterDefinition>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            StreamProcessor streamProcessor,
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
            where TFilterDefinition : IFilterDefinition
        {
            try
            {
                var runningDispatcher = dispatcher.Accept(new FilterRegistrationResponse(), cancellationToken);
                await streamProcessor.Initialize().ConfigureAwait(false);
                await ValidateFilter(
                    scopeId,
                    filterDefinition,
                    getFilterProcessor,
                    cancellationToken).ConfigureAwait(false);
                return new[] { streamProcessor.Start(), runningDispatcher };
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.Warning(
                        ex,
                        "Error occurred while trying to start Filter '{FilterId}'",
                        filterDefinition.TargetStream);
                }

                return ex;
            }
        }

        Try<StreamProcessor> TryRegisterStreamProcessor<TFilterDefinition>(
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
            {
                try
                {
                    return (_streamProcessors.TryRegister(
                        scopeId,
                        filterDefinition.TargetStream.Value,
                        new EventLogStreamDefinition(),
                        () => getFilterProcessor(),
                        cancellationToken,
                        out var outputtedFilterStreamProcessor), outputtedFilterStreamProcessor);
                }
                catch (Exception ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        _logger.Warning(
                            ex,
                            "Error occurred while trying to register stream processor for Filter '{FilterId}'",
                            filterDefinition.TargetStream);
                    }

                    return ex;
                }
            }

        async Task ValidateFilter<TFilterDefinition>(
            ScopeId scopeId,
            TFilterDefinition filterDefinition,
            Func<IFilterProcessor<TFilterDefinition>> getFilterProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
        {
            var filterValidationResults = await _filterForAllTenants.Validate(getFilterProcessor, cancellationToken).ConfigureAwait(false);

            if (filterValidationResults.Any(_ => !_.Value.Succeeded))
            {
                var firstFailedValidation = filterValidationResults.Select(_ => _.Value).First(_ => !_.Succeeded);
                _logger.Warning("Failed to register Filter: {filterId}. Filter validation failed. {reason}", filterDefinition.TargetStream, firstFailedValidation.FailureReason);
                throw new FilterValidationFailed(filterDefinition.TargetStream, firstFailedValidation.FailureReason);
            }

            var filteredStreamDefinition = new StreamDefinition(filterDefinition);
            await _streamDefinitions.Persist(scopeId, filteredStreamDefinition, cancellationToken).ConfigureAwait(false);
        }

        bool TryGetException(IEnumerable<Task> tasks, out Exception exception)
        {
            exception = tasks.FirstOrDefault(_ => _.Exception != default)?.Exception;
            if (exception != default)
            {
                while (exception.InnerException != null) exception = exception.InnerException;
            }

            return exception != default;
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
