// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters.EventHorizon;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using static Dolittle.Runtime.Events.Processing.Contracts.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    public class FiltersService : FiltersBase
    {
        readonly IHostApplicationLifetime _hostApplicationLifetime;
        readonly IStreamProcessors _streamProcessors;
        readonly IValidateFilterForAllTenants _filterForAllTenants;
        readonly IExecutionContextManager _executionContextManager;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly FactoryFor<IWriteEventsToPublicStreams> _getEventsToPublicStreamsWriter;
        readonly IStreamDefinitions _streamDefinitions;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="filterForAllTenants">The <see cref="IValidateFilterForAllTenants" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="streamDefinitions">The <see cref="IFilterDefinitions" />.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="getEventsToPublicStreamsWriter">The <see cref="FactoryFor{T}" /> for <see cref="IWriteEventsToPublicStreams" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public FiltersService(
            IHostApplicationLifetime hostApplicationLifetime,
            IStreamProcessors streamProcessors,
            IValidateFilterForAllTenants filterForAllTenants,
            IExecutionContextManager executionContextManager,
            IReverseCallDispatchers reverseCallDispatchers,
            IStreamDefinitions streamDefinitions,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            FactoryFor<IWriteEventsToPublicStreams> getEventsToPublicStreamsWriter,
            ILoggerFactory loggerFactory)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _streamProcessors = streamProcessors;
            _filterForAllTenants = filterForAllTenants;
            _executionContextManager = executionContextManager;
            _reverseCallDispatchers = reverseCallDispatchers;
            _streamDefinitions = streamDefinitions;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _getEventsToPublicStreamsWriter = getEventsToPublicStreamsWriter;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<FiltersService>();
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<FilterClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            _logger.LogDebug("Connecting Unpartitioned Filter");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
            var cancellationToken = cts.Token;
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

            if (await RejectIfNotReceivedArguments(dispatcher, cancellationToken).ConfigureAwait(false)) return;

            var arguments = dispatcher.Arguments;
            var executionContext = arguments.CallContext.ExecutionContext.ToExecutionContext();
            _logger.LogTrace("Setting execution context{NewLine}{ExecutionContext}", System.Environment.NewLine, executionContext);
            _executionContextManager.CurrentFor(arguments.CallContext.ExecutionContext);

            StreamId filterId = arguments.FilterId.ToGuid();
            ScopeId scopeId = arguments.ScopeId.ToGuid();
            var sourceStreamId = StreamId.EventLog;
            if (await RejectIfInvalidFilterId(dispatcher, filterId, cancellationToken).ConfigureAwait(false)) return;

            var filterDefinition = new FilterDefinition(sourceStreamId, filterId, false);
            await RegisterFilter(
                dispatcher,
                scopeId,
                filterDefinition,
                () => new FilterProcessor(
                    scopeId,
                    filterDefinition,
                    dispatcher,
                    _getEventsToStreamsWriter(),
                    _loggerFactory.CreateLogger<FilterProcessor>()),
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task ConnectPartitioned(
            IAsyncStreamReader<PartitionedFilterClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            _logger.LogDebug("Connecting Partitioned Filter");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
            var cancellationToken = cts.Token;
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

            if (await RejectIfNotReceivedArguments(dispatcher, cancellationToken).ConfigureAwait(false)) return;

            var arguments = dispatcher.Arguments;
            var executionContext = arguments.CallContext.ExecutionContext.ToExecutionContext();
            _logger.LogTrace("Setting execution context{NewLine}{ExecutionContext}", System.Environment.NewLine, executionContext);
            _executionContextManager.CurrentFor(arguments.CallContext.ExecutionContext);

            StreamId filterId = arguments.FilterId.ToGuid();
            ScopeId scopeId = arguments.ScopeId.ToGuid();
            var sourceStreamId = StreamId.EventLog;
            if (await RejectIfInvalidFilterId(dispatcher, filterId, cancellationToken).ConfigureAwait(false)) return;

            var filterDefinition = new FilterDefinition(sourceStreamId, filterId, true);

            await RegisterFilter(
                dispatcher,
                scopeId,
                filterDefinition,
                () => new Partitioned.FilterProcessor(
                    scopeId,
                    filterDefinition,
                    dispatcher,
                    _getEventsToStreamsWriter(),
                    _loggerFactory.CreateLogger<Partitioned.FilterProcessor>()),
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task ConnectPublic(
            IAsyncStreamReader<PublicFilterClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            _logger.LogDebug("Connecting Public Filter");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
            var cancellationToken = cts.Token;
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

            if (await RejectIfNotReceivedArguments(dispatcher, cancellationToken).ConfigureAwait(false)) return;

            var arguments = dispatcher.Arguments;
            var executionContext = arguments.CallContext.ExecutionContext.ToExecutionContext();
            _logger.LogTrace("Setting execution context{NewLine}{ExecutionContext}", System.Environment.NewLine, executionContext);
            _executionContextManager.CurrentFor(arguments.CallContext.ExecutionContext);

            StreamId filterId = arguments.FilterId.ToGuid();
            var scopeId = ScopeId.Default;
            var sourceStreamId = StreamId.EventLog;

            if (await RejectIfInvalidFilterId(dispatcher, filterId, cancellationToken).ConfigureAwait(false)) return;

            var filterDefinition = new PublicFilterDefinition(sourceStreamId, filterId);
            await RegisterFilter(
                dispatcher,
                scopeId,
                filterDefinition,
                () => new PublicFilterProcessor(
                    filterDefinition,
                    dispatcher,
                    _getEventsToPublicStreamsWriter(),
                    _loggerFactory.CreateLogger<PublicFilterProcessor>()),
                cancellationToken).ConfigureAwait(false);
        }

        async Task<bool> RejectIfNotReceivedArguments<TClientMessage, TConnectRequest, TResponse>(
            IReverseCallDispatcher<TClientMessage, FilterRuntimeToClientMessage, TConnectRequest, FilterRegistrationResponse, FilterEventRequest, TResponse> dispatcher,
            CancellationToken cancellationToken)
            where TClientMessage : IMessage, new()
            where TConnectRequest : class
            where TResponse : class
        {
            _logger.LogTrace("Waiting for connection arguments...");
            if (!await dispatcher.ReceiveArguments(cancellationToken).ConfigureAwait(false))
            {
                const string message = "Connection arguments were not received";
                _logger.LogWarning(message);
                var failure = new Failure(FiltersFailures.NoFilterRegistrationReceived, message);
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                return true;
            }

            _logger.LogTrace("Received connection arguments");
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
                _logger.LogWarning("Filter: '{Filter}' is an invalid Stream Id", filterId);
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
            _logger.LogTrace("Received Source Stream '{SourceStream}'", filterDefinition.SourceStream);
            _logger.LogTrace("Received Filter '{Filter}'", filterDefinition.TargetStream);
            _logger.LogTrace("Received Scope '{Scope}'", scopeId);
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(externalCancellationToken);
            var cancellationToken = linkedTokenSource.Token;

            _logger.LogDebug("Connecting Filter '{Filter}'", filterDefinition.TargetStream);

            var tryRegisterFilter = TryRegisterStreamProcessor(scopeId, filterDefinition, getFilterProcessor, cancellationToken);
            if (!tryRegisterFilter.Success)
            {
                linkedTokenSource.Cancel();
                if (tryRegisterFilter.HasException)
                {
                    var exception = tryRegisterFilter.Exception;
                    _logger.LogWarning(exception, "An error occurred while registering Filter '{Filter}'", filterDefinition.TargetStream);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.LogWarning("Failed to register Filter '{Filter}'. Filter already registered", filterDefinition.TargetStream);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Filter '{filterDefinition.TargetStream}'. Filter already registered.");
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
                linkedTokenSource.Cancel();
                if (tryStartFilter.HasException)
                {
                    var exception = tryStartFilter.Exception;
                    _logger.LogWarning(exception, "An error occurred while starting Filter '{Filter}' in Scope '{Scope}'", filterDefinition.TargetStream, scopeId);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.LogWarning("Could not start Filter '{Filter}' in Scope '{Scope}'", filterDefinition.TargetStream, scopeId);
                    return;
                }
            }

            var tasks = tryStartFilter.Result;
            try
            {
                await Task.WhenAny(tasks).ConfigureAwait(false);
                if (TryGetException(tasks, out var ex))
                {
                    _logger.LogWarning(ex, "An error occurred while running Filter '{Filter}' in Scope '{Scope}'", filterDefinition.TargetStream, scopeId);
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
            finally
            {
                linkedTokenSource.Cancel();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                _logger.LogDebug("Filter '{Filter}' in Scope '{Scope}' stopped", filterDefinition.TargetStream, scopeId);
            }
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
            _logger.LogDebug("Starting Filter '{Filter}'", filterDefinition.TargetStream);
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
                    _logger.LogWarning(
                        ex,
                        "Error occurred while trying to start Filter '{Filter}'",
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
                _logger.LogDebug("Registering stream processor for Filter '{Filter}' on Source Stream {SourceStream}", filterDefinition.TargetStream, filterDefinition.SourceStream);
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
                        _logger.LogWarning(
                            ex,
                            "Error occurred while trying to register stream processor for Filter '{Filter}'",
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
            _logger.LogDebug("Validating Filter '{Filter}'", filterDefinition.TargetStream);
            var filterValidationResults = await _filterForAllTenants.Validate(getFilterProcessor, cancellationToken).ConfigureAwait(false);

            if (filterValidationResults.Any(_ => !_.Value.Succeeded))
            {
                var firstFailedValidation = filterValidationResults.Select(_ => _.Value).First(_ => !_.Succeeded);
                _logger.LogWarning("Failed to register Filter '{Filter}'. Filter validation failed. {Reason}", filterDefinition.TargetStream, firstFailedValidation.FailureReason);
                throw new FilterValidationFailed(filterDefinition.TargetStream, firstFailedValidation.FailureReason);
            }

            var filteredStreamDefinition = new StreamDefinition(filterDefinition);
            _logger.LogDebug("Persisting definition for Stream '{Stream}'", filteredStreamDefinition.StreamId);
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
