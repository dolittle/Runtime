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
    public class EventHandler : IDisposable
    {
        readonly IStreamProcessors _streamProcessors;
        readonly IValidateFilterForAllTenants _filterValidator;
        readonly IStreamDefinitions _streamDefinitions;
        readonly ReverseCallDispatcherType _dispatcher;
        readonly EventHandlerRegistrationArguments _arguments;
        readonly TypeFilterWithEventSourcePartitionDefinition _filterDefinition;
        readonly FactoryFor<IWriteEventsToStreams> _getEventsToStreamsWriter;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly StreamDefinition _filteredStreamDefinition;


        bool _disposed;

        StreamProcessor _filterStreamProcessor;
        StreamProcessor _eventProcessorStreamProcessor;

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
            _filterValidator = filterForAllTenants;
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

            _filteredStreamDefinition = new StreamDefinition(
                            new TypeFilterWithEventSourcePartitionDefinition(
                                    TargetStream,
                                    TargetStream,
                                    EventTypes,
                                    Partitioned));
        }

        public StreamId TargetStream => _arguments.EventHandler.Value;
        public ScopeId Scope => _arguments.Scope.Value;
        public EventProcessorId EventProcessor => _arguments.EventHandler.Value;

        public IEnumerable<ArtifactId> EventTypes => _arguments.EventTypes;
        public bool Partitioned => _arguments.Partitioned;

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _filterStreamProcessor.Dispose();
                }

                _disposed = true;
            }
        }

        public async Task RegisterAndStart()
        {
            if (await RejectIfNonWriteableStream().ConfigureAwait(false)) return;

            _logger.LogDebug($"Connecting Event Handler '{EventProcessor.Value}'");

            await RegisterFilterStreamProcessor().ConfigureAwait(false);
            await RegisterEventProcessorStreamProcessor().ConfigureAwait(false);
            await Start().ConfigureAwait(false);
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

        async Task RegisterFilterStreamProcessor()
        {
            var streamProcessor = RegisterFilterStreamProcessor<TypeFilterWithEventSourcePartitionDefinition>();
            if (!streamProcessor.Success)
            {
                if (streamProcessor.HasException)
                {
                    var exception = streamProcessor.Exception;
                    _logger.ErrorWhileRegisteringEventHandler(exception, EventProcessor);
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
                else
                {
                    _logger.EventHandlerAlreadyRegistered(EventProcessor);
                    await Fail(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {EventProcessor.Value}. Filter already registered.").ConfigureAwait(false);
                }
            }

            _filterStreamProcessor = streamProcessor.Result;
        }

        async Task RegisterEventProcessorStreamProcessor()
        {
            _logger.RegisteringStreamProcessorForEventProcessor(EventProcessor, TargetStream);
            try
            {
                var success = _streamProcessors.TryRegister(
                    Scope,
                    EventProcessor,
                    _filteredStreamDefinition,
                    GetEventProcessor,
                    _cancellationTokenSource.Token,
                    out var streamProcessor);

                if (!success)
                {
                    _logger.EventHandlerAlreadyRegisteredOnSourceStream(EventProcessor);
                    await Fail(
                        FiltersFailures.FailedToRegisterFilter,
                        $"Failed to register Event Handler: {EventProcessor.Value}. Event Processor already registered on Source Stream: '{EventProcessor.Value}'").ConfigureAwait(false);
                }
                else
                {
                    _eventProcessorStreamProcessor = streamProcessor;
                }
            }
            catch (Exception ex)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    _logger.ErrorWhileRegisteringStreamProcessorForEventProcessor(ex, EventProcessor);
                }
                _logger.ErrorWhileRegisteringEventHandler(ex, EventProcessor);
                ExceptionDispatchInfo.Capture(ex).Throw();
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

        async Task Start()
        {
            _logger.StartingEventHandler(_filterDefinition.TargetStream);
            try
            {
                var runningDispatcher = _dispatcher.Accept(new EventHandlerRegistrationResponse(), _cancellationTokenSource.Token);
                await _filterStreamProcessor.Initialize().ConfigureAwait(false);
                await _eventProcessorStreamProcessor.Initialize().ConfigureAwait(false);
                await ValidateFilter().ConfigureAwait(false);

                var tasks = new[] { _filterStreamProcessor.Start(), _eventProcessorStreamProcessor.Start(), runningDispatcher };

                try
                {
                    await Task.WhenAny(tasks).ConfigureAwait(false);

                    if (tasks.TryGetFirstInnerMostException(out var ex))
                    {
                        _logger.ErrorWhileRunningEventHandler(ex, EventProcessor, Scope);
                        ExceptionDispatchInfo.Capture(ex).Throw();
                    }
                }
                finally
                {
                    _cancellationTokenSource.Cancel();
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    _logger.EventHandlerDisconnected(EventProcessor, Scope);
                }
            }
            catch (Exception ex)
            {
                _cancellationTokenSource.Cancel();
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    _logger.ErrorWhileStartingEventHandler(ex, EventProcessor, Scope);
                }
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        async Task ValidateFilter()
        {
            _logger.ValidatingFilter(_filterDefinition.TargetStream);
            var filterValidationResults = await _filterValidator.Validate(GetFilterProcessor, _cancellationTokenSource.Token).ConfigureAwait(false);

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

        async Task Fail(FailureId failureId, string message)
        {
            var failure = new Failure(failureId, message);
            await _dispatcher.Reject(new EventHandlerRegistrationResponse { Failure = failure }, _cancellationTokenSource.Token).ConfigureAwait(false);
        }
    }
}
