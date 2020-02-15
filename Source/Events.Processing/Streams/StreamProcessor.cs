// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;

#pragma warning disable CA2008

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a system that processes a stream of events.
    /// </summary>
    public class StreamProcessor : IDisposable
    {
        readonly IEventProcessor _processor;
        readonly ILogger _logger;
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly IStreamProcessorStateRepository _streamProcessorStateRepository;
        readonly string _logMessagePrefix;
        readonly CancellationTokenSource _cancellationTokenSource;
        Task _task;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessor"/> class.
        /// </summary>
        /// <param name="tenantId">The <see cref="TenantId"/> the <see cref="StreamProcessor"/> belongs to.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> of the source stream.</param>
        /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
        /// <param name="streamProcessorStateRepository">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="eventsFromStreamsFetcher">The<see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
        public StreamProcessor(
            TenantId tenantId,
            StreamId sourceStreamId,
            IEventProcessor processor,
            IStreamProcessorStateRepository streamProcessorStateRepository,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            ILogger logger)
        {
            _processor = processor;
            _logger = logger;
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _streamProcessorStateRepository = streamProcessorStateRepository;
            Identifier = new StreamProcessorId(_processor.Identifier, sourceStreamId);
            _logMessagePrefix = $"Stream Partition Processor for event processor '{Identifier.EventProcessorId}' with source stream '{Identifier.SourceStreamId}' for tenant '{tenantId}'";
            CurrentState = StreamProcessorState.New;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Gets the <see cref="StreamProcessorId">identifier</see> for the <see cref="StreamProcessor"/>.
        /// </summary>
        public StreamProcessorId Identifier { get; }

        /// <summary>
        /// Gets the <see cref="EventProcessorId" />.
        /// </summary>
        public EventProcessorId EventProcessorId => _processor.Identifier;

        /// <summary>
        /// Gets the current <see cref="StreamProcessorState" />.
        /// </summary>
        public StreamProcessorState CurrentState { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Start processing.
        /// </summary>
        public void Start()
        {
            _task = Task.Factory.StartNew(BeginProcessing, TaskCreationOptions.DenyChildAttach);
        }

        /// <summary>
        /// Stop processing.
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Starts up the <see cref="StreamProcessor" />.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task BeginProcessing()
        {
            try
            {
                CurrentState = await GetPersistedCurrentState().ConfigureAwait(false);
                do
                {
                    await CatchupFailingPartitions().ConfigureAwait(false);

                    StreamEvent eventAndPartition = default;
                    while (eventAndPartition == default && !_cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            eventAndPartition = await FetchNextEventWithPartitionToProcess().ConfigureAwait(false);
                        }
                        catch (NoEventInStreamAtPosition)
                        {
                            await Task.Delay(1000).ConfigureAwait(false);
                        }
                    }

                    if (CurrentState.FailingPartitions.Keys.Contains(eventAndPartition.Partition))
                    {
                        CurrentState = await IncrementPosition().ConfigureAwait(false);
                    }
                    else
                    {
                        var processingResult = await ProcessEvent(eventAndPartition.Event, eventAndPartition.Partition).ConfigureAwait(false);
                        if (processingResult.Succeeded)
                        {
                            var currentState = await IncrementPosition().ConfigureAwait(false);
                            CurrentState = currentState;
                        }
                        else if (processingResult is IRetryProcessingResult retryProcessingResult)
                        {
                            CurrentState = await AddFailingPartitionAndIncrementPosition(eventAndPartition.PartitionId, retryProcessingResult.RetryTimeout, retryProcessingResult.FailureReason).ConfigureAwait(false);
                        }
                        else
                        {
                            CurrentState = await AddFailingPartitionAndIncrementPosition(eventAndPartition.PartitionId, DateTimeOffset.MaxValue, processingResult.FailureReason).ConfigureAwait(false);
                        }
                    }
                }
                while (!_cancellationTokenSource.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _logger.Error($"{_logMessagePrefix} failed - {ex}");
                }
            }
        }

        async Task CatchupFailingPartitions()
        {
            var failingPartitions = CurrentState.FailingPartitions.ToList();
            foreach (var kvp in failingPartitions)
            {
                var partitionId = kvp.Key;
                var failingPartitionState = kvp.Value;
                if (ShouldRetryProcessing(failingPartitionState))
                {
                    var nextPosition = await FindPositionOfNextEventInPartition(partitionId, failingPartitionState.Position).ConfigureAwait(false);
                    while (ShouldProcessNextEventInPartition(nextPosition))
                    {
                        if (!ShouldRetryProcessing(failingPartitionState)) break;

                        var eventAndPartition = await FetchEventWithPartitionAtPosition(nextPosition).ConfigureAwait(false);
                        var processingResult = await ProcessEvent(eventAndPartition.Event, eventAndPartition.Partition).ConfigureAwait(false);

                        if (processingResult.Succeeded)
                        {
                            failingPartitionState = await ChangePositionInFailingPartition(partitionId, failingPartitionState.Position, nextPosition.Increment()).ConfigureAwait(false);
                        }
                        else if (processingResult is IRetryProcessingResult retryProcessingResult)
                        {
                            failingPartitionState = await SetFailingPartitionState(partitionId, retryProcessingResult.RetryTimeout, processingResult.FailureReason, nextPosition).ConfigureAwait(false);
                        }
                        else
                        {
                            failingPartitionState = await SetFailingPartitionState(partitionId, DateTimeOffset.MaxValue, processingResult.FailureReason, nextPosition).ConfigureAwait(false);
                        }

                        nextPosition = await FindPositionOfNextEventInPartition(partitionId, failingPartitionState.Position).ConfigureAwait(false);
                    }

                    if (ShouldRetryProcessing(failingPartitionState)) CurrentState = await RemoveFailingPartition(partitionId).ConfigureAwait(false);
                }
            }
        }

        Task<StreamProcessorState> AddFailingPartitionAndIncrementPosition(PartitionId partitionId, uint retryTimeout, string reason) => AddFailingPartitionAndIncrementPosition(partitionId, DateTimeOffset.UtcNow.AddMilliseconds(retryTimeout), reason);

        async Task<StreamProcessorState> AddFailingPartitionAndIncrementPosition(PartitionId partitionId, DateTimeOffset retryTime, string reason)
        {
            _logger.Debug($"{_logMessagePrefix} is adding failing partition '{partitionId}' with retry time '{retryTime}'");
            await _streamProcessorStateRepository.AddFailingPartition(Identifier, partitionId, CurrentState.Position, retryTime, reason).ConfigureAwait(false);
            return await IncrementPosition().ConfigureAwait(false);
        }

        Task<StreamProcessorState> RemoveFailingPartition(PartitionId partitionId)
        {
            _logger.Debug($"{_logMessagePrefix} is removing failing partition '{partitionId}'");
            return _streamProcessorStateRepository.RemoveFailingPartition(Identifier, partitionId);
        }

        Task<StreamProcessorState> GetPersistedCurrentState()
        {
            _logger.Debug($"{_logMessagePrefix} is getting the persisted state for this stream processor.");
            return _streamProcessorStateRepository.GetOrAddNew(Identifier);
        }

        Task<IProcessingResult> ProcessEvent(Store.CommittedEvent @event, PartitionId partitionId)
        {
            _logger.Debug($"{_logMessagePrefix} is processing event '{@event.Type.Id.Value}' in partition '{partitionId.Value}'");
            return _processor.Process(@event, partitionId);
        }

        Task<StreamProcessorState> IncrementPosition()
        {
            _logger.Debug($"{_logMessagePrefix} is incrementing its position from '{CurrentState.Position.Value}' to '{CurrentState.Position.Increment().Value}'");
            return _streamProcessorStateRepository.IncrementPosition(Identifier);
        }

        async Task<FailingPartitionState> ChangePositionInFailingPartition(PartitionId partitionId, StreamPosition oldPosition, StreamPosition newPosition)
        {
            _logger.Debug($"{_logMessagePrefix} is chaning its position from '{oldPosition.Value}' to '{newPosition.Value}' in partition'{partitionId.Value}'");
            var newFailingPartitionState = new FailingPartitionState { Position = newPosition, RetryTime = DateTimeOffset.MinValue };
            CurrentState = await _streamProcessorStateRepository.SetFailingPartitionState(
                Identifier,
                partitionId,
                newFailingPartitionState).ConfigureAwait(false);
            return newFailingPartitionState;
        }

        Task<StreamEvent> FetchNextEventWithPartitionToProcess() => FetchEventWithPartitionAtPosition(CurrentState.Position);

        Task<StreamEvent> FetchEventWithPartitionAtPosition(StreamPosition position)
        {
            _logger.Debug($"{_logMessagePrefix} is fetching event at position '{position.Value}'.");
            return _eventsFromStreamsFetcher.Fetch(Identifier.SourceStreamId, position);
        }

        Task<StreamPosition> FindPositionOfNextEventInPartition(PartitionId partitionId, StreamPosition fromPosition)
        {
            _logger.Debug($"{_logMessagePrefix} is fetching next event to process in partition '{partitionId}' from position '{fromPosition}'.");
            return _eventsFromStreamsFetcher.FindNext(Identifier.SourceStreamId, partitionId, fromPosition);
        }

        Task<FailingPartitionState> SetFailingPartitionState(PartitionId partitionId, uint retryTimeout, string reason, StreamPosition position) => SetFailingPartitionState(partitionId, DateTimeOffset.UtcNow.AddMilliseconds(retryTimeout), reason, position);

        async Task<FailingPartitionState> SetFailingPartitionState(PartitionId partitionId, DateTimeOffset retryTime, string reason, StreamPosition position)
        {
            _logger.Debug($"{_logMessagePrefix} is setting retry time '{retryTime}' and position '{position.Value}' for partition '{partitionId.Value}'");
            var newFailingPartitionState = new FailingPartitionState { Position = position, RetryTime = retryTime, Reason = reason };
            CurrentState = await _streamProcessorStateRepository.SetFailingPartitionState(
                Identifier,
                partitionId,
                newFailingPartitionState)
                .ConfigureAwait(false);

            return newFailingPartitionState;
        }

        bool ShouldProcessNextEventInPartition(StreamPosition position) => position.Value < CurrentState.Position.Value;

        bool ShouldRetryProcessing(FailingPartitionState state) => DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
    }
}