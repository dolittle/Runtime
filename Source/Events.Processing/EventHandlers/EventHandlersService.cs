// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Grpc.Core;
using static Dolittle.Runtime.Events.Processing.Contracts.EventHandlers;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents the implementation of <see cref="EventHandlersBase"/>.
    /// </summary>
    public class EventHandlersService : EventHandlersBase
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly IValidateFilterForAllTenants _filterForAllTenants;
        readonly IStreamProcessors _streamProcessors;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly FactoryFor<IStreamDefinitionRepository> _getStreamDefinitionRepository;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerManager _loggerManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
        /// </summary>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="filterForAllTenants">The <see cref="IValidateFilterForAllTenants" />.</param>
        /// <param name="streamProcessors">The <see cref="IStreamProcessors" />.</param>
        /// <param name="getEventsToStreamsWriter">The <see cref="FactoryFor{T}" /> <see cref="IWriteEventsToStreams" />.</param>
        /// <param name="getStreamDefinitionRepository">The <see cref="FactoryFor{T}" /> <see cref="IStreamDefinitionRepository" />.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager"/>.</param>
        public EventHandlersService(
            IPerformActionOnAllTenants onAllTenants,
            IValidateFilterForAllTenants filterForAllTenants,
            IStreamProcessors streamProcessors,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            FactoryFor<IStreamDefinitionRepository> getStreamDefinitionRepository,
            IReverseCallDispatchers reverseCallDispatchers,
            IExecutionContextManager executionContextManager,
            ILoggerManager loggerManager)
        {
            _onAllTenants = onAllTenants;
            _filterForAllTenants = filterForAllTenants;
            _streamProcessors = streamProcessors;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _getStreamDefinitionRepository = getStreamDefinitionRepository;
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
            var streamDefinition = new StreamDefinition(filterDefinition);
            Func<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>> getFilterProcessor = () => new TypeFilterWithEventSourcePartition(
                    scopeId,
                    filterDefinition,
                    _getEventsToStreamsWriter(),
                    _loggerManager.CreateLogger<TypeFilterWithEventSourcePartition>());
            var successfullyRegisteredFilterStreamProcessor = _streamProcessors.TryRegister(
                scopeId,
                eventHandlerId,
                streamDefinition,
                () => getFilterProcessor(),
                context.CancellationToken,
                out var filterStreamProcessor);
            var successfullyRegisteredEventProcessorStreamProcessor = _streamProcessors.TryRegister(
                scopeId,
                eventHandlerId,
                streamDefinition,
                () => new EventProcessor(
                    scopeId,
                    eventHandlerId,
                    dispatcher,
                    _loggerManager.CreateLogger<EventProcessor>()),
                context.CancellationToken,
                out var eventProcessorStreamProcessor);

            if (!successfullyRegisteredFilterStreamProcessor || !successfullyRegisteredEventProcessorStreamProcessor)
            {
                _logger.Warning("Failed during registration of Event Handler: '{eventHandlerId}'. Already registered", eventHandlerId);
                var failure = new Failure(
                    EventHandlersFailures.FailedToRegisterEventHandler,
                    $"Failed during registration of Event Handler: '{eventHandlerId}'. Already registered");

                await WriteFailedRegistrationResponse(dispatcher, failure, context.CancellationToken).ConfigureAwait(false);
                return;
            }

            Task runningDispatcher;
            try
            {
                await filterStreamProcessor.Initialize().ConfigureAwait(false);
                await eventProcessorStreamProcessor.Initialize().ConfigureAwait(false);
                runningDispatcher = dispatcher.Accept(new EventHandlerRegistrationResponse(), context.CancellationToken);
                var filterValidationResults = await _filterForAllTenants.Validate(getFilterProcessor, context.CancellationToken).ConfigureAwait(false);

                if (filterValidationResults.Any(_ => !_.Value.Succeeded))
                {
                    var firstFailedValidation = filterValidationResults.Select(_ => _.Value).First(_ => !_.Succeeded);
                    _logger.Warning("Failed to register EventHandler: {eventHandlerId}. Filter validation failed. {reason}", eventHandlerId, firstFailedValidation.FailureReason);
                    throw new FilterValidationFailed(filterDefinition.TargetStream, firstFailedValidation.FailureReason);
                }

                await _onAllTenants.PerformAsync(_ => _getStreamDefinitionRepository().Persist(scopeId, streamDefinition, context.CancellationToken)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error while registering Event Handler: {eventHandlerId}", eventHandlerId);
                filterStreamProcessor.Unregister();
                eventProcessorStreamProcessor.Unregister();
                throw;
            }

            try
            {
                await Task.WhenAny(filterStreamProcessor.Start(), eventProcessorStreamProcessor.Start(), runningDispatcher).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    _logger.Debug(ex, "Event Handler: '{eventHandlerId}' stopped", eventHandlerId);
                }
            }
            finally
            {
                _logger.Debug("Event Handler: '{eventHandlerId}' stopped", eventHandlerId);
            }
        }

        Task WriteFailedRegistrationResponse(
            IReverseCallDispatcher<EventHandlersClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlersRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse> dispatcher,
            Failure failure,
            CancellationToken cancellationToken) => dispatcher.Reject(new EventHandlerRegistrationResponse { Failure = failure }, cancellationToken);
    }
}