// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Async;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Services;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using static Dolittle.Runtime.Events.Processing.Contracts.EventHandlers;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents the implementation of <see cref="EventHandlersBase"/>.
    /// </summary>
    public class EventHandlersService : EventHandlersBase
    {
        readonly IValidateFilterForAllTenants _filterForAllTenants;
        readonly IStreamProcessors _streamProcessors;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly IStreamDefinitions _streamDefinitions;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerManager _loggerManager;
        readonly ILogger _logger;
        readonly IHostApplicationLifetime _hostApplicationLifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
        /// </summary>
        /// <param name="hostApplicationLifetime">The <see cref="IHostApplicationLifetime" />.</param>
        /// <param name="filterForAllTenants">The <see cref="IValidateFilterForAllTenants" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="streamDefinitions">The<see cref="IStreamDefinitions" />.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager"/>.</param>
        public EventHandlersService(
            IHostApplicationLifetime hostApplicationLifetime,
            IValidateFilterForAllTenants filterForAllTenants,
            IStreamProcessors streamProcessors,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            IStreamDefinitions streamDefinitions,
            IReverseCallDispatchers reverseCallDispatchers,
            IExecutionContextManager executionContextManager,
            ILoggerManager loggerManager)
        {
            _filterForAllTenants = filterForAllTenants;
            _streamProcessors = streamProcessors;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _streamDefinitions = streamDefinitions;
            _reverseCallDispatchers = reverseCallDispatchers;
            _executionContextManager = executionContextManager;
            _loggerManager = loggerManager;
            _logger = loggerManager.CreateLogger<EventHandlersService>();
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<EventHandlerClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<EventHandlerRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            _logger.Debug("Connecting Event Handler");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping, context.CancellationToken);
            var cancellationToken = cts.Token;
            var dispatcher = _reverseCallDispatchers.GetFor<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>(
                runtimeStream,
                clientStream,
                context,
                _ => _.RegistrationRequest,
                (serverMessage, registrationResponse) => serverMessage.RegistrationResponse = registrationResponse,
                (serverMessage, request) => serverMessage.HandleRequest = request,
                _ => _.HandleResult,
                _ => _.CallContext,
                (request, context) => request.CallContext = context,
                _ => _.CallContext,
                (message, ping) => message.Ping = ping,
                message => message.Pong);
            _logger.Trace("Waiting for connection arguments...");
            if (!await dispatcher.ReceiveArguments(cancellationToken).ConfigureAwait(false))
            {
                const string message = "Event Handlers connection arguments were not received";
                _logger.Warning(message);
                var failure = new Failure(EventHandlersFailures.NoEventHandlerRegistrationReceived, message);
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                return;
            }

            _logger.Trace("Received connection arguments");
            var executionContext = dispatcher.Arguments.CallContext.ExecutionContext.ToExecutionContext();
            _logger.Trace("Setting execution context{NewLine}{ExecutionContext}", System.Environment.NewLine, executionContext);
            _executionContextManager.CurrentFor(executionContext);

            var arguments = dispatcher.Arguments;
            var sourceStream = StreamId.EventLog;
            _logger.Trace("Received Source Stream Id '{SourceStream}'", sourceStream);
            var eventHandlerId = arguments.EventHandlerId.To<EventProcessorId>();
            _logger.Trace("Received Event Handler Id '{EventHandler}'", eventHandlerId);
            StreamId targetStream = eventHandlerId.Value;
            var scopeId = arguments.ScopeId.To<ScopeId>();
            _logger.Trace("Received Scope Id '{Scope}'", scopeId);
            var types = arguments.Types_.Select(_ => _.Id.To<ArtifactId>());
            _logger.Trace("Received Types: [{Types}]'", string.Join(", ", types.Select(_ => $"'{_}'")));
            var partitioned = arguments.Partitioned;
            _logger.Trace("Event Handler '{EventHandler}' {PartitionedString}", eventHandlerId, partitioned ? "is partitioned" : "is not partitioned");
            if (targetStream.IsNonWriteable)
            {
                _logger.Warning("Cannot register Event Handler '{EventHandler}' because it is an invalid Stream Id", eventHandlerId);
                var failure = new Failure(
                    EventHandlersFailures.CannotRegisterEventHandlerOnNonWriteableStream,
                    $"Cannot register Event Handler: '{eventHandlerId}' because it is an invalid Stream Id");
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                return;
            }

            _logger.Debug("Connecting Event Handler '{EventHandlerId}'", eventHandlerId);
            var filterDefinition = new TypeFilterWithEventSourcePartitionDefinition(sourceStream, targetStream, types, partitioned);
            var filteredStreamDefinition = new StreamDefinition(filterDefinition);
            Func<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>> getFilterProcessor = () => new TypeFilterWithEventSourcePartition(
                    scopeId,
                    filterDefinition,
                    _getEventsToStreamsWriter(),
                    _loggerManager.CreateLogger<TypeFilterWithEventSourcePartition>());
            var tryRegisterFilterStreamProcessor = TryRegisterFilterStreamProcessor<TypeFilterWithEventSourcePartitionDefinition>(
                scopeId,
                eventHandlerId,
                getFilterProcessor,
                cancellationToken);

            if (!tryRegisterFilterStreamProcessor.Success)
            {
                if (tryRegisterFilterStreamProcessor.HasException)
                {
                    var exception = tryRegisterFilterStreamProcessor.Exception;
                    _logger.Warning(exception, "An error occurred while registering Event Handler '{EventHandlerId}'", eventHandlerId);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.Debug("Failed to register Event Handler '{EventHandlerId}'. Filter already registered", eventHandlerId);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {eventHandlerId}. Filter already registered.");
                    await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            using var filterStreamProcessor = tryRegisterFilterStreamProcessor.Result;

            var tryRegisterEventProcessorStreamProcessor = TryRegisterEventProcessorStreamProcessor(
                scopeId,
                eventHandlerId,
                filteredStreamDefinition,
                () => new EventProcessor(
                    scopeId,
                    eventHandlerId,
                    dispatcher,
                    _loggerManager.CreateLogger<EventProcessor>()),
                cancellationToken);

            if (!tryRegisterEventProcessorStreamProcessor.Success)
            {
                if (tryRegisterEventProcessorStreamProcessor.HasException)
                {
                    var exception = tryRegisterEventProcessorStreamProcessor.Exception;
                    _logger.Warning(exception, "An error occurred while registering Event Handler' {EventHandlerId}", eventHandlerId);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.Warning("Failed to register Event Handler: {eventHandlerId}. Event Processor already registered on Source Stream '{SourceStreamId}'", eventHandlerId, eventHandlerId);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {eventHandlerId}. Event Processor already registered on Source Stream: '{eventHandlerId}'");
                    await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            using var eventProcessorStreamProcessor = tryRegisterEventProcessorStreamProcessor.Result;

            var tryStartEventHandler = await TryStartEventHandler(
                dispatcher,
                filterStreamProcessor,
                eventProcessorStreamProcessor,
                scopeId,
                filterDefinition,
                getFilterProcessor,
                cancellationToken).ConfigureAwait(false);
            if (!tryStartEventHandler.Success)
            {
                cts.Cancel();
                if (tryStartEventHandler.HasException)
                {
                    var exception = tryStartEventHandler.Exception;
                    _logger.Debug(exception, "An error occurred while starting Event Handler '{EventHandlerId}' in Scope {ScopeId}", eventHandlerId, scopeId);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.Warning("Could not start Event Handler '{EventHandlerId}' in Scope: {ScopeId}", eventHandlerId, scopeId);
                    return;
                }
            }

            var tasks = tryStartEventHandler.Result;
            try
            {
                await Task.WhenAny(tasks).ConfigureAwait(false);

                if (TryGetException(tasks, out var ex))
                {
                    _logger.Warning(ex, "An error occurred while running Event Handler '{EventHandlerId}' in Scope: '{ScopeId}'", eventHandlerId, scopeId);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
            finally
            {
                cts.Cancel();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                _logger.Debug("Event Handler: '{EventHandler}' in Scope: '{ScopeId}' disconnected", eventHandlerId, scopeId);
            }
        }

        async Task<Try<IEnumerable<Task>>> TryStartEventHandler<TClientMessage, TConnectRequest, TResponse, TFilterDefinition>(
            IReverseCallDispatcher<TClientMessage, EventHandlerRuntimeToClientMessage, TConnectRequest, EventHandlerRegistrationResponse, HandleEventRequest, TResponse> dispatcher,
            StreamProcessor filterStreamProcessor,
            StreamProcessor eventProcessorStreamProcessor,
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
                _logger.Debug("Starting Event Handler '{EventHandlerId}'", filterDefinition.TargetStream);
                var runningDispatcher = dispatcher.Accept(new EventHandlerRegistrationResponse(), cancellationToken);
                await filterStreamProcessor.Initialize().ConfigureAwait(false);
                await eventProcessorStreamProcessor.Initialize().ConfigureAwait(false);
                await ValidateFilter(
                    scopeId,
                    filterDefinition,
                    getFilterProcessor,
                    cancellationToken).ConfigureAwait(false);
                return new[] { filterStreamProcessor.Start(), eventProcessorStreamProcessor.Start(), runningDispatcher };
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.Warning(
                        ex,
                        "Error occurred while trying to start Event Handler '{EventHandlerId}'",
                        filterDefinition.TargetStream);
                }

                return ex;
            }
        }

        Try<StreamProcessor> TryRegisterFilterStreamProcessor<TFilterDefinition>(
            ScopeId scopeId,
            EventProcessorId eventHandlerId,
            Func<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>> getFilterProcessor,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
            {
                _logger.Debug("Registering stream processor for Filter '{Filter}' on Event Log", eventHandlerId);
                try
                {
                    return (_streamProcessors.TryRegister(
                        scopeId,
                        eventHandlerId,
                        new EventLogStreamDefinition(),
                        getFilterProcessor,
                        cancellationToken,
                        out var outputtedFilterStreamProcessor), outputtedFilterStreamProcessor);
                }
                catch (Exception ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        _logger.Warning(
                            ex,
                            "Error occurred while trying to register stream processor for Filter '{Filter}'",
                            eventHandlerId);
                    }

                    return ex;
                }
            }

        Try<StreamProcessor> TryRegisterEventProcessorStreamProcessor(
            ScopeId scopeId,
            EventProcessorId eventHandlerId,
            IStreamDefinition streamDefinition,
            Func<IEventProcessor> getEventProcessor,
            CancellationToken cancellationToken)
            {
                _logger.Debug("Registering stream processor for Event Processor '{EventProcessor}' on Stream '{SourceStream}'", eventHandlerId, streamDefinition.StreamId);
                try
                {
                    return (_streamProcessors.TryRegister(
                        scopeId,
                        eventHandlerId,
                        streamDefinition,
                        getEventProcessor,
                        cancellationToken,
                        out var outputtedEventProcessorStreamProcessor), outputtedEventProcessorStreamProcessor);
                }
                catch (Exception ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        _logger.Warning(
                            ex,
                            "Error occurred while trying to register stream processor for Event Processor '{EventProcessor}'",
                            eventHandlerId);
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
            _logger.Debug("Validating Filter '{Filter}'", filterDefinition.TargetStream);
            var filterValidationResults = await _filterForAllTenants.Validate(getFilterProcessor, cancellationToken).ConfigureAwait(false);

            if (filterValidationResults.Any(_ => !_.Value.Succeeded))
            {
                var firstFailedValidation = filterValidationResults.Select(_ => _.Value).First(_ => !_.Succeeded);
                _logger.Warning("Filter validation failed. {Reason}", filterDefinition.TargetStream, firstFailedValidation.FailureReason);
                throw new FilterValidationFailed(filterDefinition.TargetStream, firstFailedValidation.FailureReason);
            }

            var filteredStreamDefinition = new StreamDefinition(filterDefinition);
            _logger.Debug("Persisting definition for Stream '{Stream}'", filteredStreamDefinition.StreamId);
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

        Task WriteFailedRegistrationResponse(
            IReverseCallDispatcher<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse> dispatcher,
            Failure failure,
            CancellationToken cancellationToken) => dispatcher.Reject(new EventHandlerRegistrationResponse { Failure = failure }, cancellationToken);
    }
}