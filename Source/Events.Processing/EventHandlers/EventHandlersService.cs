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

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
        /// </summary>
        /// <param name="filterForAllTenants">The <see cref="IValidateFilterForAllTenants" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="streamDefinitions">The<see cref="IStreamDefinitions" />.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager"/>.</param>
        public EventHandlersService(
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
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<EventHandlersClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<EventHandlerRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            var dispatcher = _reverseCallDispatchers.GetFor<EventHandlersClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlersRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>(
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

            _executionContextManager.CurrentFor(dispatcher.Arguments.CallContext.ExecutionContext);
            var arguments = dispatcher.Arguments;

            var sourceStream = StreamId.EventLog;
            var eventHandlerId = arguments.EventHandlerId.To<EventProcessorId>();
            StreamId targetStream = eventHandlerId.Value;
            var scopeId = arguments.ScopeId.To<ScopeId>();
            var types = arguments.Types_.Select(_ => _.Id.To<ArtifactId>());
            var partitioned = arguments.Partitioned;
            if (targetStream.IsNonWriteable)
            {
                _logger.Warning("Cannot register Event Handler: '{eventHandlerId}' because it is an invalid Stream Id", eventHandlerId);
                var failure = new Failure(
                    EventHandlersFailures.CannotRegisterEventHandlerOnNonWriteableStream,
                    $"Cannot register Event Handler: '{eventHandlerId}' because it is an invalid Stream Id");
                await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                return;
            }

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
                context.CancellationToken);

            if (!tryRegisterFilterStreamProcessor.Success)
            {
                if (tryRegisterFilterStreamProcessor.HasException)
                {
                    var exception = tryRegisterFilterStreamProcessor.Exception;
                    _logger.Warning(exception, "An error occurred while registering Event Handler: {eventHandlerId}", eventHandlerId);
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                }
                else
                {
                    _logger.Debug("Failed to register Event Handler: {eventHandlerId}. Filter already registered", eventHandlerId);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {eventHandlerId}. Filter already registered.");
                    await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
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
                context.CancellationToken);

            if (!tryRegisterEventProcessorStreamProcessor.Success)
            {
                if (tryRegisterEventProcessorStreamProcessor.HasException)
                {
                    var exception = tryRegisterEventProcessorStreamProcessor.Exception;
                    _logger.Warning(exception, "An error occurred while registering Event Handler: {eventHandlerId}", eventHandlerId);
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                }
                else
                {
                    _logger.Debug("Failed to register Event Handler: {eventHandlerId}. Event Processor already registered on Source Stream: '{sourceStreamId}'", eventHandlerId, eventHandlerId);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {eventHandlerId}. Event Processor already registered on Source Stream: '{eventHandlerId}'");
                    await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            using var eventProcessorStreamProcessor = tryRegisterEventProcessorStreamProcessor.Result;

            using var internalCancellationTokenSource = new CancellationTokenSource();
            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(internalCancellationTokenSource.Token, context.CancellationToken);
            var cancellationToken = linkedTokenSource.Token;

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
                internalCancellationTokenSource.Cancel();
                if (tryStartEventHandler.HasException)
                {
                    var exception = tryStartEventHandler.Exception;
                    _logger.Debug(exception, "An error occurred while starting Event Handler: '{eventHandlerId}' in Scope: {scopeId}", eventHandlerId, scopeId);
                    ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                }
                else
                {
                    _logger.Debug("Could not start Event Handler: '{eventHandlerId}' in Scope: {scopeId}", eventHandlerId, scopeId);
                    return;
                }
            }

            var tasks = tryStartEventHandler.Result;
            var anyTask = await Task.WhenAny(tasks).ConfigureAwait(false);
            if (TryGetException(tasks, out var ex))
            {
                internalCancellationTokenSource.Cancel();
                _logger.Warning(ex, "An error occurred while processing Event Handler: '{eventHandlerId}' in Scope: '{scopeId}'", eventHandlerId, scopeId);
                await Task.WhenAll(tasks).ConfigureAwait(false);
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            if (!context.CancellationToken.IsCancellationRequested)
            {
                _logger.Warning(ex, "Event Handler: '{eventHandler}' in Scope: '{scopeId}' failed", eventHandlerId, scopeId);
            }

            _logger.Debug("Event Handler: '{eventHandler}' in Scope: '{scopeId}' stopped", eventHandlerId, scopeId);
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

        Task WriteFailedRegistrationResponse(
            IReverseCallDispatcher<EventHandlersClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlersRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse> dispatcher,
            Failure failure,
            CancellationToken cancellationToken) => dispatcher.Reject(new EventHandlerRegistrationResponse { Failure = failure }, cancellationToken);
    }
}