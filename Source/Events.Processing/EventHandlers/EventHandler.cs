// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using ReverseCallDispatcherType = Dolittle.Runtime.Services.IReverseCallDispatcher<
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
                                    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    public class EventHandler
    {
        readonly IStreamProcessors _streamProcessors;
        readonly IValidateFilterForAllTenants _filterForAllTenants;
        readonly IStreamDefinitions _streamDefinitions;
        readonly ReverseCallDispatcherType _dispatcher;
        readonly EventHandlerRegistrationArguments _arguments;
        readonly TypeFilterWithEventSourcePartitionDefinition _filterDefinition;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;
        readonly CancellationTokenSource _cancellationTokenSource;

        public EventHandler(
            IStreamProcessors streamProcessors,
            IValidateFilterForAllTenants filterForAllTenants,
            IStreamDefinitions streamDefinitions,
            ReverseCallDispatcherType dispatcher,
            EventHandlerRegistrationArguments arguments,
            FactoryFor<IWriteEventsToStreams> getEventsToStreamsWriter,
            ILoggerFactory loggerFactory,
            CancellationTokenSource cancellationTokenSource)
        {
            _logger = loggerFactory.CreateLogger<EventHandler>();
            _streamProcessors = streamProcessors;
            _filterForAllTenants = filterForAllTenants;
            _streamDefinitions = streamDefinitions;
            _dispatcher = dispatcher;
            _arguments = arguments;
            _getEventsToStreamsWriter = getEventsToStreamsWriter;
            _loggerFactory = loggerFactory;
            _cancellationTokenSource = cancellationTokenSource;
            _filterDefinition = new TypeFilterWithEventSourcePartitionDefinition(
                StreamId.EventLog,
                TargetStream,
                arguments.EventTypes,
                arguments.Partitioned);
        }

        public StreamId TargetStream => _arguments.EventHandler.Value;
        public ScopeId Scope => _arguments.Scope.Value;
        public EventProcessorId EventProcessor => _arguments.EventHandler.Value;

        public IEnumerable<ArtifactId> EventTypes => _arguments.EventTypes;
        public bool Partitioned => _arguments.Partitioned;

        public async Task RegisterAndStart()
        {
            if (await RejectIfNonWriteableStream().ConfigureAwait(false)) return;

            _logger.LogDebug($"Connecting Event Handler '{EventProcessor.Value}'");

            var filterStreamProcessor = await RegisterFilterStreamProcessor().ConfigureAwait(false);
            using (filterStreamProcessor.Result)
            {
                var eventProcessorStreamProcessor = await RegisterEventProcessorStreamProcessor().ConfigureAwait(false);
                using (eventProcessorStreamProcessor.Result)
                {
                    var tasks = await StartEventHandler(filterStreamProcessor, eventProcessorStreamProcessor).ConfigureAwait(false);
                    try
                    {
                        await Task.WhenAny(tasks.Result).ConfigureAwait(false);

                        if (tasks.Result.TryGetFirstInnerMostException(out var ex))
                        {
                            _logger.ErrorWhileRunningEventHandler(ex, EventProcessor, Scope);
                            ExceptionDispatchInfo.Capture(ex).Throw();
                        }
                    }
                    finally
                    {
                        _cancellationTokenSource.Cancel();
                        await Task.WhenAll(tasks.Result).ConfigureAwait(false);
                        _logger.EventHandlerDisconnected(EventProcessor, Scope);
                    }
                }
            }
        }

        async Task<bool> RejectIfNonWriteableStream()
        {
            if (TargetStream.IsNonWriteable)
            {
                _logger.EventHandlerIsInvalid(EventProcessor);
                var failure = new Failure(
                    EventHandlersFailures.CannotRegisterEventHandlerOnNonWriteableStream,
                    $"Cannot register Event Handler: '{EventProcessor.Value}' because it is an invalid Stream Id");
                await _dispatcher.Reject(new EventHandlerRegistrationResponse { Failure = failure }, _cancellationTokenSource.Token).ConfigureAwait(false);
                return true;
            }
            return false;
        }

        async Task<Try<StreamProcessor>> RegisterFilterStreamProcessor()
        {
            var tryRegisterFilterStreamProcessor = RegisterFilterStreamProcessor<TypeFilterWithEventSourcePartitionDefinition>();
            if (!tryRegisterFilterStreamProcessor.Success)
            {
                if (tryRegisterFilterStreamProcessor.HasException)
                {
                    var exception = tryRegisterFilterStreamProcessor.Exception;
                    _logger.ErrorWhileRegisteringEventHandler(exception, EventProcessor);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.EventHandlerAlreadyRegistered(EventProcessor);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {EventProcessor.Value}. Filter already registered.");
                    await _dispatcher.Reject(new EventHandlerRegistrationResponse { Failure = failure }, _cancellationTokenSource.Token).ConfigureAwait(false);
                }
            }

            return tryRegisterFilterStreamProcessor;
        }

        async Task<Try<StreamProcessor>> RegisterEventProcessorStreamProcessor()
        {
            // This should be the stream definition of the filtered stream for an event processor to use
            var filteredStreamDefinition = new StreamDefinition(
                new TypeFilterWithEventSourcePartitionDefinition(
                        TargetStream,
                        TargetStream,
                        EventTypes,
                        Partitioned));

            var tryRegisterEventProcessorStreamProcessor = TryRegisterEventProcessorStreamProcessor(filteredStreamDefinition);

            if (!tryRegisterEventProcessorStreamProcessor.Success)
            {
                if (tryRegisterEventProcessorStreamProcessor.HasException)
                {
                    var exception = tryRegisterEventProcessorStreamProcessor.Exception;
                    _logger.ErrorWhileRegisteringEventHandler(exception, EventProcessor);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.EventHandlerAlreadyRegisteredOnSourceStream(EventProcessor);
                    var failure = new Failure(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {EventProcessor.Value}. Event Processor already registered on Source Stream: '{EventProcessor.Value}'");
                    await _dispatcher.Reject(new EventHandlerRegistrationResponse { Failure = failure }, _cancellationTokenSource.Token).ConfigureAwait(false);
                }
            }

            return tryRegisterEventProcessorStreamProcessor;
        }

        Try<StreamProcessor> TryRegisterEventProcessorStreamProcessor(IStreamDefinition sourceStreamDefinition)
        {
            _logger.RegisteringStreamProcessorForEventProcessor(EventProcessor, sourceStreamDefinition.StreamId);
            try
            {
                return (_streamProcessors.TryRegister(
                    Scope,
                    EventProcessor,
                    sourceStreamDefinition,
                    GetEventProcessor,
                    _cancellationTokenSource.Token,
                    out var outputtedEventProcessorStreamProcessor), outputtedEventProcessorStreamProcessor);
            }
            catch (Exception ex)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    _logger.ErrorWhileRegisteringStreamProcessorForEventProcessor(ex, EventProcessor);
                }

                return ex;
            }
        }

        Try<StreamProcessor> RegisterFilterStreamProcessor<TFilterDefinition>()
            where TFilterDefinition : IFilterDefinition
        {
            _logger.RegisteringStreamProcessorForFilter(EventProcessor);
            try
            {
                return (_streamProcessors.TryRegister(
                    Scope,
                    EventProcessor,
                    new EventLogStreamDefinition(),
                    GetFilterProcessor,
                    _cancellationTokenSource.Token,
                    out var outputtedFilterStreamProcessor), outputtedFilterStreamProcessor);
            }
            catch (Exception ex)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _logger.ErrorWhileRegisteringStreamProcessorForFilter(ex, EventProcessor);
                }

                return ex;
            }
        }

        async Task<Try<IEnumerable<Task>>> StartEventHandler(
            StreamProcessor filterStreamProcessor,
            StreamProcessor eventProcessorStreamProcessor)
        {
            var tryStartEventHandler = await TryStartEventHandler(filterStreamProcessor, eventProcessorStreamProcessor).ConfigureAwait(false);
            if (!tryStartEventHandler.Success)
            {
                _cancellationTokenSource.Cancel();
                if (tryStartEventHandler.HasException)
                {
                    var exception = tryStartEventHandler.Exception;
                    _logger.ErrorWhileStartingEventHandler(exception, EventProcessor, Scope);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.CouldNotStartEventHandler(EventProcessor, Scope);
                }
            }

            return tryStartEventHandler;
        }

        async Task<Try<IEnumerable<Task>>> TryStartEventHandler(
            StreamProcessor filterStreamProcessor,
            StreamProcessor eventProcessorStreamProcessor)
        {
            _logger.StartingEventHandler(_filterDefinition.TargetStream);
            try
            {
                var runningDispatcher = _dispatcher.Accept(new EventHandlerRegistrationResponse(), _cancellationTokenSource.Token);
                await filterStreamProcessor.Initialize().ConfigureAwait(false);
                await eventProcessorStreamProcessor.Initialize().ConfigureAwait(false);
                await ValidateFilter().ConfigureAwait(false);
                return new[] { filterStreamProcessor.Start(), eventProcessorStreamProcessor.Start(), runningDispatcher };
            }
            catch (Exception ex)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    _logger.ErrorWhileStartingEventHandler(ex, _filterDefinition.TargetStream, Scope);
                }

                return ex;
            }
        }

        async Task ValidateFilter()
        {
            _logger.ValidatingFilter(_filterDefinition.TargetStream);
            var filterValidationResults = await _filterForAllTenants.Validate(GetFilterProcessor, _cancellationTokenSource.Token).ConfigureAwait(false);

            if (filterValidationResults.Any(_ => !_.Value.Succeeded))
            {
                var firstFailedValidation = filterValidationResults.First(_ => !_.Value.Succeeded).Value;
                _logger.FilterValidationFailed(_filterDefinition.TargetStream, firstFailedValidation.FailureReason);
                throw new FilterValidationFailed(_filterDefinition.TargetStream, firstFailedValidation.FailureReason);
            }

            var filteredStreamDefinition = new StreamDefinition(_filterDefinition);
            _logger.PersistingStreamDefinition(filteredStreamDefinition.StreamId);
            await _streamDefinitions.Persist(Scope, filteredStreamDefinition, _cancellationTokenSource.Token).ConfigureAwait(false);
        }

        IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> GetFilterProcessor()
        {
            return new TypeFilterWithEventSourcePartition(
                    Scope,
                    _filterDefinition,
                    _getEventsToStreamsWriter(),
                    _loggerFactory.CreateLogger<TypeFilterWithEventSourcePartition>());
        }

        IEventProcessor GetEventProcessor()
        {
            return new EventProcessor(
                    Scope,
                    EventProcessor,
                    _dispatcher,
                    _loggerFactory.CreateLogger<EventProcessor>());
        }
    }
}
