// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using static Dolittle.Runtime.Events.Processing.Contracts.Projections;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents the implementation of <see cref="ProjectionsBase"/>.
    /// </summary>
    public class ProjectionsService : ProjectionsBase
    {
        readonly IValidateFilterForAllTenants _filterForAllTenants;
        readonly IStreamProcessors _streamProcessors;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly IStreamDefinitions _streamDefinitions;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerFactory _loggerFactory;
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
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public EventHandlersService(
            IHostApplicationLifetime hostApplicationLifetime,
            IValidateFilterForAllTenants filterForAllTenants,
            IStreamProcessors streamProcessors,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            IStreamDefinitions streamDefinitions,
            IReverseCallDispatchers reverseCallDispatchers,
            IExecutionContextManager executionContextManager,
            ILoggerFactory loggerFactory)
        {
            _filterForAllTenants = filterForAllTenants;
            _streamProcessors = streamProcessors;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _streamDefinitions = streamDefinitions;
            _reverseCallDispatchers = reverseCallDispatchers;
            _executionContextManager = executionContextManager;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ProjectionsService>();
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<ProjectionsClientToRuntimeMessage> runtimeStream,
            IServerStreamWriter<ProjectionsRuntimeToClientMessage> clientStream,
            ServerCallContext context)
        {
            _logger.LogDebug("Connecting Event Handler");
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
            _logger.LogTrace("Waiting for connection arguments...");
            if (!await dispatcher.ReceiveArguments(cancellationToken).ConfigureAwait(false))
            {
                const string message = "Event Handlers connection arguments were not received";
                _logger.LogWarning(message);
                var failure = new Failure(EventHandlersFailures.NoEventHandlerRegistrationReceived, message);
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                return;
            }

            _logger.LogTrace("Received connection arguments");
            var arguments = dispatcher.Arguments;
            var executionContext = arguments.CallContext.ExecutionContext.ToExecutionContext();
            _logger.SettingExecutionContext(executionContext);
            _executionContextManager.CurrentFor(executionContext);

            var sourceStream = StreamId.EventLog;
            EventProcessorId eventHandlerId = arguments.EventHandlerId.ToGuid();
            StreamId targetStream = eventHandlerId.Value;
            ScopeId scopeId = arguments.ScopeId.ToGuid();
            var types = arguments.Types_.Select(_ => new ArtifactId(_.Id.ToGuid()));
            var partitioned = arguments.Partitioned;
            _logger.ReceivedEventHandler(sourceStream, eventHandlerId, scopeId, types, partitioned);
            if (targetStream.IsNonWriteable)
            {
                _logger.EventHandlerIsInvalid(eventHandlerId);
                var failure = new Failure(
                    EventHandlersFailures.CannotRegisterEventHandlerOnNonWriteableStream,
                    $"Cannot register Event Handler: '{eventHandlerId.Value}' because it is an invalid Stream Id");
                await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                return;
            }

            _logger.LogDebug("Connecting Event Handler '{EventHandlerId}'", eventHandlerId.Value);
            var filterDefinition = new TypeFilterWithEventSourcePartitionDefinition(sourceStream, targetStream, types, partitioned);

            Func<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>> getFilterProcessor = () => new TypeFilterWithEventSourcePartition(
                    scopeId,
                    filterDefinition,
                    _getEventsToStreamsWriter(),
                    _loggerFactory.CreateLogger<TypeFilterWithEventSourcePartition>());
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
                    _logger.ErrorWhileRegisteringEventHandler(exception, eventHandlerId);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.EventHandlerAlreadyRegistered(eventHandlerId);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {eventHandlerId.Value}. Filter already registered.");
                    await WriteFailedRegistrationResponse(dispatcher, failure, cancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            using var filterStreamProcessor = tryRegisterFilterStreamProcessor.Result;

            // This should be the stream definition of the filtered stream for an event processor to use
            var filteredStreamDefinition = new StreamDefinition(new TypeFilterWithEventSourcePartitionDefinition(targetStream, targetStream, types, partitioned));
            var tryRegisterEventProcessorStreamProcessor = TryRegisterEventProcessorStreamProcessor(
                scopeId,
                eventHandlerId,
                filteredStreamDefinition,
                () => new EventProcessor(
                    scopeId,
                    eventHandlerId,
                    dispatcher,
                    _loggerFactory.CreateLogger<EventProcessor>()),
                cancellationToken);

            if (!tryRegisterEventProcessorStreamProcessor.Success)
            {
                if (tryRegisterEventProcessorStreamProcessor.HasException)
                {
                    var exception = tryRegisterEventProcessorStreamProcessor.Exception;
                    _logger.ErrorWhileRegisteringEventHandler(exception, eventHandlerId);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.EventHandlerAlreadyRegisteredOnSourceStream(eventHandlerId);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {eventHandlerId}. Event Processor already registered on Source Stream: '{eventHandlerId.Value}'");
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
                    _logger.ErrorWhileStartingEventHandler(exception, eventHandlerId, scopeId);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.CouldNotStartEventHandler(eventHandlerId, scopeId);
                    return;
                }
            }

            var tasks = tryStartEventHandler.Result;
            try
            {
                await Task.WhenAny(tasks).ConfigureAwait(false);

                if (TryGetException(tasks, out var ex))
                {
                    _logger.ErrorWhileRunningEventHandler(ex, eventHandlerId, scopeId);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
            finally
            {
                cts.Cancel();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                _logger.EventHandlerDisconnected(eventHandlerId, scopeId);
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
            _logger.StartingEventHandler(filterDefinition.TargetStream);
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
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.ErrorWhileStartingEventHandler(ex, filterDefinition.TargetStream, scopeId);
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
            _logger.RegisteringStreamProcessorForFilter(eventHandlerId);
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
                    _logger.ErrorWhileRegisteringStreamProcessorForFilter(ex, eventHandlerId);
                }

                return ex;
            }
        }

        Try<StreamProcessor> TryRegisterEventProcessorStreamProcessor(
            ScopeId scopeId,
            EventProcessorId eventHandlerId,
            IStreamDefinition sourceStreamDefinition,
            Func<IEventProcessor> getEventProcessor,
            CancellationToken cancellationToken)
        {
            _logger.RegisteringStreamProcessorForEventProcessor(eventHandlerId, sourceStreamDefinition.StreamId);
            try
            {
                return (_streamProcessors.TryRegister(
                    scopeId,
                    eventHandlerId,
                    sourceStreamDefinition,
                    getEventProcessor,
                    cancellationToken,
                    out var outputtedEventProcessorStreamProcessor), outputtedEventProcessorStreamProcessor);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.ErrorWhileRegisteringStreamProcessorForEventProcessor(ex, eventHandlerId);
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
            _logger.ValidatingFilter(filterDefinition.TargetStream);
            var filterValidationResults = await _filterForAllTenants.Validate(getFilterProcessor, cancellationToken).ConfigureAwait(false);

            if (filterValidationResults.Any(_ => !_.Value.Succeeded))
            {
                var firstFailedValidation = filterValidationResults.Select(_ => _.Value).First(_ => !_.Succeeded);
                _logger.FilterValidationFailed(filterDefinition.TargetStream, firstFailedValidation.FailureReason);
                throw new FilterValidationFailed(filterDefinition.TargetStream, firstFailedValidation.FailureReason);
            }

            var filteredStreamDefinition = new StreamDefinition(filterDefinition);
            _logger.PersistingStreamDefinition(filteredStreamDefinition.StreamId);
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
