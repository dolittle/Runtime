// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Processes an individual <see cref="CommittedEventEnvelope" /> for the correct <see cref="TenantId" />.
    /// </summary>
    public class StreamProcessor
    {
        const int TimeToWait = 1000;
        readonly TenantId _tenant;
        readonly IEventProcessorNew _processor;
        readonly ILogger _logger;
        readonly FactoryFor<IFetchNextEvent> _getNextEventFetcher;
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="tenant">The <see cref="TenantId" /> that this processor is scoped to.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="processor">An <see cref="IEventProcessorNew" /> to process the event.</param>
        /// <param name="getStreamProcessorStateRepository">A factory function to return a correctly scoped instance of <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="getNextEventFetcher">A factory function to return a correctly scoped instance of <see cref="IFetchNextEvent" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        public StreamProcessor(
            TenantId tenant,
            StreamId sourceStreamId,
            IEventProcessorNew processor,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStateRepository,
            FactoryFor<IFetchNextEvent> getNextEventFetcher,
            ILogger logger)
        {
            _tenant = tenant;
            _processor = processor;
            _logger = logger;
            _getNextEventFetcher = getNextEventFetcher;
            _getStreamProcessorStateRepository = getStreamProcessorStateRepository;
            Key = new StreamProcessorKey(_processor.Identifier, sourceStreamId);
            LogMessageBeginning = $"Stream Processor for tenant '{tenant.Value}', event processor '{Key.EventProcessorId.Value}' with source stream '{Key.SourceStreamId.Value}'";
        }

        /// <summary>
        /// Gets the unique identifer for the <see cref="IEventProcessor" />.
        /// </summary>
        public StreamProcessorKey Key { get; }

        /// <summary>
        /// Gets the current <see cref="StreamProcessorState" />.
        /// </summary>
        public StreamProcessorState CurrentState { get; private set; }

        string LogMessageBeginning { get; }

        bool RetryClockIsTicking { get; } = false;

        /// <summary>
        /// Starts up the processing of the stream. It will continiously process and wait for events to process until it is stopped.
        /// </summary>
        public async void Start()
        {
            try
            {
                _logger.Debug($"{LogMessageBeginning} is starting up.");
                using (var repository = _getStreamProcessorStateRepository())
                {
                    CurrentState = repository.Get(Key) ?? StreamProcessorState.New;
                }

                if (IsWaiting()) SetState(StreamProcessingState.Processing);
                while (!IsStopping())
                {
                    if (IsProcessing()) await EnterProcessing().ConfigureAwait(false);
                    else if (IsRetrying()) await EnterRetrying().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug($"{LogMessageBeginning}: Error while processing - {ex}");
            }

            _logger.Debug($"{LogMessageBeginning} has stopped processing.");
        }

        async Task EnterProcessing()
        {
            try
            {
                _logger.Debug($"{LogMessageBeginning} is starting to process.");
                while (IsProcessing())
                {
                    var @event = await FetchNextEvent().ConfigureAwait(false);
                    if (@event == null)
                    {
                        _logger.Debug($"{LogMessageBeginning} has no event to process.");
                        Wait();
                    }
                    else
                    {
                        await ProcessEvent(@event).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{LogMessageBeginning}: Error while processing - {ex}");
                throw;
            }
        }

        async Task EnterRetrying()
        {
            try
            {
                // TODO: Start clock
                // TODO: Store retry_timeout in state
                _logger.Warning($"{LogMessageBeginning} processing failed. Entering retrying state.");
                var @event = await FetchNextEvent().ConfigureAwait(false);
                while (IsRetrying())
                {
                    _logger.Debug($"{LogMessageBeginning} is waiting to retry");
                    Thread.Sleep(TimeToWait);
                    _logger.Debug($"{LogMessageBeginning} is trying to process event with artifact id '{@event.Metadata.Artifact.Id.Value}' again.");
                    await ProcessEvent(@event).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{LogMessageBeginning}: Error while retrying - {ex}");
                throw;
            }
        }

        async Task<CommittedEventEnvelope> FetchNextEvent()
        {
            try
            {
                _logger.Debug($"{LogMessageBeginning} is fetching next event.");
                var nextEventFetcher = _getNextEventFetcher();
                return await nextEventFetcher.FetchNextEvent(Key.SourceStreamId, CurrentState.Position).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error($"{LogMessageBeginning}: Error while fetching next event - {ex}");
                throw;
            }
        }

        async Task ProcessEvent(CommittedEventEnvelope @event)
        {
            try
            {
                _logger.Debug($"{LogMessageBeginning} is processing event with artifact id '{@event.Metadata.Artifact.Id}'");
                SetState(StreamProcessingState.Processing);
                var processingResult = await _processor.Process(@event).ConfigureAwait(false);

                HandleProcessingResult(@event, processingResult);
            }
            catch (Exception ex)
            {
                _logger.Error($"{LogMessageBeginning}: Error processing event with artifact id '{@event.Metadata.Artifact.Id.Value} - {ex.Message}");
                throw;
            }
        }

        void HandleProcessingResult(CommittedEventEnvelope @event, IProcessingResult processingResult)
        {
            switch (processingResult.Value)
            {
                case ProcessingResultValue.Succeeded:
                    _logger.Debug($"{LogMessageBeginning} processed event with artifact id '{@event.Metadata.Artifact.Id}'");
                    IncrementPosition();
                    break;
                case ProcessingResultValue.Retry:
                    SetState(StreamProcessingState.Retrying);
                    break;
                case ProcessingResultValue.Failed:
                    Stop();
                    break;
            }
        }

        void Wait(int milliseconds = TimeToWait)
        {
            _logger.Debug($"{LogMessageBeginning} is waiting...");
            SetState(StreamProcessingState.Waiting);
            Thread.Sleep(milliseconds);
            SetState(StreamProcessingState.Processing);
        }

        void Stop()
        {
            _logger.Error($"{LogMessageBeginning} is stopping");
            SetState(StreamProcessingState.Stopping);
        }

        void IncrementPosition()
        {
            _logger.Debug($"{LogMessageBeginning} is incrementing its position in the source stream '{Key.SourceStreamId.Value}'");
            SetState(StreamProcessingState.Processing, CurrentState.Position.Increment());
        }

        void SetState(StreamProcessingState state) => SetState(state, CurrentState.Position);

        void SetState(StreamProcessingState state, StreamPosition position)
        {
            if (IsInState(state, position)) return;
            try
            {
                using (var repository = _getStreamProcessorStateRepository())
                {
                    _logger.Debug($"{LogMessageBeginning} is setting new state to {CurrentState}");
                    repository.Set(Key, CurrentState);
                    CurrentState = new StreamProcessorState(state, position);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{LogMessageBeginning}: Error setting new state - {ex}");

                // This is a weird scenario where the state cannot be persisted in the event store.
                CurrentState = new StreamProcessorState(StreamProcessingState.Stopping, CurrentState.Position);
                throw;
            }
        }

        bool IsProcessing() => IsInProcessingState(StreamProcessingState.Processing);

        bool IsWaiting() => IsInProcessingState(StreamProcessingState.Waiting);

        bool IsRetrying() => IsInProcessingState(StreamProcessingState.Retrying);

        bool IsStopping() => IsInProcessingState(StreamProcessingState.Stopping);

        bool IsInProcessingState(StreamProcessingState state) => CurrentState.State == state;

        bool IsInState(StreamProcessingState state, StreamPosition position) => state == CurrentState.State && position == CurrentState.Position;
    }
}