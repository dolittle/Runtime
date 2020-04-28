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
        readonly IStreamProcessorStates _streamProcessorStates;
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailingPartitions"/> class.
        /// </summary>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStates" />.</param>
        /// <param name="eventsFromStreamsFetcher">The <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public FailingPartitions(
            IStreamProcessorStates streamProcessorStates,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            ILogger<FailingPartitions> logger)
        {
            _streamProcessorStates = streamProcessorStates;
            _logger = logger;
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
        }

        /// <inheritdoc/>
        public async Task<StreamProcessorState> AddFailingPartitionFor(StreamProcessorId streamProcessorId, StreamProcessorState oldState, StreamPosition failedPosition, PartitionId partition, DateTimeOffset retryTime, string reason, CancellationToken cancellationToken)
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
        public async Task<StreamProcessorState> CatchupFor(StreamProcessorId streamProcessorId, IEventProcessor eventProcessor, StreamProcessorState streamProcessorState, CancellationToken cancellationToken)
        {
            if (streamProcessorState.FailingPartitions.Count > 0) streamProcessorState = (await _streamProcessorStates.TryGetFor(streamProcessorId, cancellationToken).ConfigureAwait(false)).streamProcessorState as StreamProcessorState;
            var failingPartitionsList = streamProcessorState.FailingPartitions.ToList();
            foreach (var kvp in failingPartitionsList)
            {
                var partition = kvp.Key;
                var failingPartitionState = kvp.Value;
                if (ShouldRetryProcessing(failingPartitionState))
                {
                    var nextPosition = await FindPositionOfNextEventInPartition(streamProcessorId, partition, failingPartitionState.Position, cancellationToken).ConfigureAwait(false);
                    while (ShouldProcessNextEventInPartition(nextPosition, streamProcessorState.Position))
                    {
                        if (!ShouldRetryProcessing(failingPartitionState)) break;

                        var streamEvent = await FetchEventAtPosition(streamProcessorId, nextPosition, cancellationToken).ConfigureAwait(false);
                        var processingResult = await ProcessEvent(failingPartitionState, eventProcessor, streamEvent.Event, partition, cancellationToken).ConfigureAwait(false);

                        if (processingResult.Succeeded)
                        {
                            (streamProcessorState, failingPartitionState) = await ChangePositionInFailingPartition(streamProcessorId, streamProcessorState, partition, nextPosition + 1).ConfigureAwait(false);
                        }
                        else if (processingResult.Retry)
                        {
                            (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(streamProcessorId, streamProcessorState, partition, failingPartitionState.ProcessingAttempts + 1, processingResult.RetryTimeout, processingResult.FailureReason, nextPosition).ConfigureAwait(false);
                        }
                        else
                        {
                            (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(streamProcessorId, streamProcessorState, partition, failingPartitionState.ProcessingAttempts + 1, DateTimeOffset.MaxValue, processingResult.FailureReason, nextPosition).ConfigureAwait(false);
                        }

                        nextPosition = await FindPositionOfNextEventInPartition(streamProcessorId, partition, failingPartitionState.Position, cancellationToken).ConfigureAwait(false);
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

        Task<StreamEvent> FetchEventAtPosition(StreamProcessorId streamProcessorId, StreamPosition position, CancellationToken cancellationToken)
        {
            return _eventsFromStreamsFetcher.Fetch(streamProcessorId.ScopeId, streamProcessorId.SourceStreamId, position, cancellationToken);
        }

        Task<StreamPosition> FindPositionOfNextEventInPartition(StreamProcessorId streamProcessorId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken)
        {
            return _eventsFromStreamsFetcher.FindNext(streamProcessorId.ScopeId, streamProcessorId.SourceStreamId, partitionId, fromPosition, cancellationToken);
        }

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

        Task<IProcessingResult> ProcessEvent(FailingPartitionState failingPartitionState, IEventProcessor eventProcessor, CommittedEvent @event, PartitionId partition, CancellationToken cancellationToken)
        {
            return eventProcessor.Process(@event, partition, failingPartitionState.Reason, failingPartitionState.ProcessingAttempts - 1, cancellationToken);
        }

        Task PersistNewState(StreamProcessorId streamProcessorId, StreamProcessorState newState) => _streamProcessorStates.Persist(streamProcessorId, newState, CancellationToken.None);

        bool ShouldProcessNextEventInPartition(StreamPosition failingPartitionPosition, StreamPosition streamProcessorPosition) =>
            failingPartitionPosition.Value < streamProcessorPosition.Value;

        bool ShouldRetryProcessing(FailingPartitionState state) =>
            DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
    }
}