// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
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
        readonly IEventProcessor _processor;
        readonly ILogger _logger;
        readonly IFetchNextEvent _nextEventFetcher;
        readonly IStreamProcessorStateRepository _streamProcessorStateRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="streamProcessorStateRepository">A factory function to return a correctly scoped instance of <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="nextEventFetcher">A factory function to return a correctly scoped instance of <see cref="IFetchNextEvent" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        public StreamProcessor(
            StreamId sourceStreamId,
            IEventProcessor processor,
            IStreamProcessorStateRepository streamProcessorStateRepository,
            IFetchNextEvent nextEventFetcher,
            ILogger logger)
        {
            _processor = processor;
            _logger = logger;
            _nextEventFetcher = nextEventFetcher;
            _streamProcessorStateRepository = streamProcessorStateRepository;
            Key = new StreamProcessorKey(_processor.Identifier, sourceStreamId);
            LogMessageBeginning = $"Stream Processor for event processor '{Key.EventProcessorId.Value}' with source stream '{Key.SourceStreamId.Value}'";
        }

        /// <summary>
        /// Gets the unique identifer for the <see cref="StreamProcessor" />.
        /// </summary>
        public StreamProcessorKey Key { get; }

        /// <summary>
        /// Gets the current <see cref="StreamProcessorState" />.
        /// </summary>
        public StreamProcessorState CurrentState { get; private set; }

        /// <summary>
        /// Gets the <see cref="EventProcessorId" />.
        /// </summary>
        public EventProcessorId EventProcessorId => _processor.Identifier;

        string LogMessageBeginning { get; }

        bool RetryClockIsTicking { get; } = false;

        /// <summary>
        /// Starts up the processing of the stream. It will continiously process and wait for events to process until it is stopped.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of starting the <see cref="StreamProcessor" />.</returns>
        public async Task Start()
        {
            try
            {
                _logger.Information($"{LogMessageBeginning} is starting up.");
                CurrentState = _streamProcessorStateRepository.Get(Key) ?? StreamProcessorState.New;
                _logger.Information($"{LogMessageBeginning} got current state '{CurrentState}' from stream processor state repository.");

                if (IsWaiting()) SetState(StreamProcessingState.Processing);
                while (!IsStopping())
                {
                    if (IsProcessing()) await EnterProcessing().ConfigureAwait(false);
                    else if (IsRetrying()) await EnterRetrying().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Information($"{LogMessageBeginning}: Error while processing - {ex}");
            }

            _logger.Information($"{LogMessageBeginning} has stopped processing.");
        }

        async Task EnterProcessing()
        {
            try
            {
                _logger.Information($"{LogMessageBeginning} is starting to process.");
                while (IsProcessing())
                {
                    var @event = await FetchNextEvent().ConfigureAwait(false);
                    if (@event == null)
                    {
                        _logger.Information($"{LogMessageBeginning} has no event to process.");
                        await Wait().ConfigureAwait(false);
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
                _logger.Warning($"{LogMessageBeginning} processing failed. Entering retrying state.");
                var @event = await FetchNextEvent().ConfigureAwait(false);
                while (IsRetrying())
                {
                    _logger.Information($"{LogMessageBeginning} is waiting to retry");
                    await Task.Delay(TimeToWait).ConfigureAwait(false);
                    _logger.Information($"{LogMessageBeginning} is trying to process event with artifact id '{@event.Metadata.Artifact.Id.Value}' again.");
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
                _logger.Information($"{LogMessageBeginning} is fetching next event.");
                return await _nextEventFetcher.FetchNextEvent(Key.SourceStreamId, CurrentState.Position).ConfigureAwait(false);
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
                _logger.Information($"{LogMessageBeginning} is processing event with artifact id '{@event.Metadata.Artifact.Id}'");
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
            if (processingResult.Succeeded)
            {
                _logger.Information($"{LogMessageBeginning} processed event with artifact id '{@event.Metadata.Artifact.Id}'");
                IncrementPosition();
            }
            else if (processingResult.Retry)
            {
                SetState(StreamProcessingState.Retrying);
            }
            else
            {
                Stop();
            }
        }

        async Task Wait(int milliseconds = TimeToWait)
        {
            _logger.Information($"{LogMessageBeginning} is waiting...");
            SetState(StreamProcessingState.Waiting);
            await Task.Delay(milliseconds).ConfigureAwait(false);
            SetState(StreamProcessingState.Processing);
        }

        void Stop()
        {
            _logger.Error($"{LogMessageBeginning} is stopping");
            SetState(StreamProcessingState.Stopping);
        }

        void IncrementPosition()
        {
            _logger.Information($"{LogMessageBeginning} is incrementing its position in the source stream '{Key.SourceStreamId.Value}'");
            SetState(StreamProcessingState.Processing, CurrentState.Position.Increment());
        }

        void SetState(StreamProcessingState state) => SetState(state, CurrentState.Position);

        void SetState(StreamProcessingState state, StreamPosition position)
        {
            if (IsInState(state, position)) return;
            try
            {
                CurrentState = new StreamProcessorState(state, position);
                _logger.Information($"{LogMessageBeginning} is setting new state to {CurrentState}");
                _streamProcessorStateRepository.Set(Key, CurrentState);
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

        bool IsInState(StreamProcessingState state, StreamPosition position) => IsInProcessingState(state) && position == CurrentState.Position;
    }
}