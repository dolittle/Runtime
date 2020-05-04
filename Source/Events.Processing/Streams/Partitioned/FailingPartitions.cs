// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned
{
    /// <summary>
    /// Represents an implementation of <see cref="IFailingPartitions" />.
    /// </summary>
    public class FailingPartitions : IFailingPartitions
    {
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IEventProcessor _eventProcessor;
        readonly ICanFetchEventsFromPartitionedStream _eventsFromStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailingPartitions"/> class.
        /// </summary>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="eventsFromStreamsFetcher">The <see cref="ICanFetchEventsFromStream" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public FailingPartitions(
            IStreamProcessorStateRepository streamProcessorStates,
            IEventProcessor eventProcessor,
            ICanFetchEventsFromPartitionedStream eventsFromStreamsFetcher,
            ILogger<FailingPartitions> logger)
        {
            _streamProcessorStates = streamProcessorStates;
            _eventProcessor = eventProcessor;
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IStreamProcessorState> AddFailingPartitionFor(StreamProcessorId streamProcessorId, StreamProcessorState oldState, StreamPosition failedPosition, PartitionId partition, DateTimeOffset retryTime, string reason, CancellationToken cancellationToken)
        {
            var failingPartition = new FailingPartitionState(failedPosition, retryTime, reason, 1);
            var failingPartitions = new Dictionary<PartitionId, FailingPartitionState>(oldState.FailingPartitions)
                {
                    [partition] = failingPartition
                };
            var newState = new StreamProcessorState(failedPosition + 1, failingPartitions);
            await PersistNewState(streamProcessorId, newState).ConfigureAwait(false);
            return newState;
        }

        /// <inheritdoc/>
        public async Task<IStreamProcessorState> CatchupFor(StreamProcessorId streamProcessorId, StreamProcessorState streamProcessorState, CancellationToken cancellationToken)
        {
            if (streamProcessorState.FailingPartitions.Count > 0) streamProcessorState = (await _streamProcessorStates.TryGetFor(streamProcessorId, cancellationToken).ConfigureAwait(false)).Result as StreamProcessorState;
            var failingPartitionsList = streamProcessorState.FailingPartitions.ToList();
            foreach (var kvp in failingPartitionsList)
            {
                var partition = kvp.Key;
                var failingPartitionState = kvp.Value;
                if (ShouldRetryProcessing(failingPartitionState))
                {
                    var streamEvent = await _eventsFromStreamsFetcher.FetchInPartition(partition, failingPartitionState.Position, cancellationToken).ConfigureAwait(false);
                    while (ShouldProcessNextEventInPartition(streamEvent.Position, streamProcessorState.Position))
                    {
                        if (!ShouldRetryProcessing(failingPartitionState)) break;

                        var processingResult = await RetryProcessingEvent(failingPartitionState, streamEvent.Event, partition, cancellationToken).ConfigureAwait(false);

                        if (processingResult.Succeeded)
                        {
                            (streamProcessorState, failingPartitionState) = await ChangePositionInFailingPartition(streamProcessorId, streamProcessorState, partition, streamEvent.Position + 1).ConfigureAwait(false);
                        }
                        else if (processingResult.Retry)
                        {
                            (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(streamProcessorId, streamProcessorState, partition, failingPartitionState.ProcessingAttempts + 1, processingResult.RetryTimeout, processingResult.FailureReason, streamEvent.Position).ConfigureAwait(false);
                        }
                        else
                        {
                            (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(streamProcessorId, streamProcessorState, partition, failingPartitionState.ProcessingAttempts + 1, DateTimeOffset.MaxValue, processingResult.FailureReason, streamEvent.Position).ConfigureAwait(false);
                        }

                        streamEvent = await _eventsFromStreamsFetcher.FetchInPartition(partition, failingPartitionState.Position, cancellationToken).ConfigureAwait(false);
                    }

                    if (ShouldRetryProcessing(failingPartitionState)) streamProcessorState = await RemoveFailingPartition(streamProcessorId, streamProcessorState, partition).ConfigureAwait(false);
                }
            }

            return streamProcessorState;
        }

        async Task<StreamProcessorState> RemoveFailingPartition(StreamProcessorId streamProcessorId, StreamProcessorState oldState, PartitionId partition)
        {
            var newFailingPartitions = oldState.FailingPartitions;
            newFailingPartitions.Remove(partition);
            var newState = new StreamProcessorState(oldState.Position, newFailingPartitions);
            oldState.FailingPartitions.Remove(partition);

            await PersistNewState(streamProcessorId, newState).ConfigureAwait(false);
            return newState;
        }

        Task<(StreamProcessorState, FailingPartitionState)> ChangePositionInFailingPartition(StreamProcessorId streamProcessorId, StreamProcessorState oldState, PartitionId partitionId, StreamPosition newPosition) =>
            SetFailingPartitionState(streamProcessorId, oldState, partitionId, 0, DateTimeOffset.MinValue, string.Empty, newPosition);

        Task<(StreamProcessorState, FailingPartitionState)> SetFailingPartitionState(StreamProcessorId streamProcessorId, StreamProcessorState oldState, PartitionId partitionId, uint processingAttempts, TimeSpan retryTimeout, string reason, StreamPosition position) =>
            SetFailingPartitionState(streamProcessorId, oldState, partitionId, processingAttempts, DateTimeOffset.UtcNow.Add(retryTimeout), reason, position);

        async Task<(StreamProcessorState, FailingPartitionState)> SetFailingPartitionState(StreamProcessorId streamProcessorId, StreamProcessorState oldState, PartitionId partitionId, uint processingAttempts, DateTimeOffset retryTime, string reason, StreamPosition position)
        {
            var newFailingPartitionState = new FailingPartitionState(position, retryTime, reason, processingAttempts);
            var newFailingPartitions = oldState.FailingPartitions;
            newFailingPartitions[partitionId] = newFailingPartitionState;
            var newState = new StreamProcessorState(oldState.Position, newFailingPartitions);

            await PersistNewState(streamProcessorId, newState).ConfigureAwait(false);

            return (newState, newFailingPartitionState);
        }

        Task<IProcessingResult> RetryProcessingEvent(FailingPartitionState failingPartitionState, CommittedEvent @event, PartitionId partition, CancellationToken cancellationToken) =>
            _eventProcessor.Process(@event, partition, failingPartitionState.Reason, failingPartitionState.ProcessingAttempts - 1, cancellationToken);

        Task PersistNewState(StreamProcessorId streamProcessorId, StreamProcessorState newState) => _streamProcessorStates.Persist(streamProcessorId, newState, CancellationToken.None);

        bool ShouldProcessNextEventInPartition(StreamPosition failingPartitionPosition, StreamPosition streamProcessorPosition) =>
            failingPartitionPosition.Value < streamProcessorPosition.Value;

        bool ShouldRetryProcessing(FailingPartitionState state) =>
            DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
    }
}