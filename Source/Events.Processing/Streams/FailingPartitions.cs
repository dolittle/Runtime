// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using grpc = contracts::Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IFailingPartitions" />.
    /// </summary>
    public class FailingPartitions : IFailingPartitions
    {
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IFetchEventsFromStreams _eventsFromStreamsFetcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailingPartitions"/> class.
        /// </summary>
        /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStateRepository" />.</param>
        /// <param name="eventsFromStreamsFetcher">The <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public FailingPartitions(
            IStreamProcessorStateRepository streamProcessorStates,
            IFetchEventsFromStreams eventsFromStreamsFetcher,
            ILogger logger)
        {
            _streamProcessorStates = streamProcessorStates;
            _logger = logger;
            _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
        }

        /// <inheritdoc/>
        public Task<StreamProcessorState> AddFailingPartitionFor(StreamProcessorId streamProcessorId, PartitionId partition, StreamPosition position, ProcessorFailureType failureType, DateTimeOffset retryTime, string reason, CancellationToken cancellationToken)
        {
            _logger.Debug($"Adding failing partition '{partition}' with retry time '{retryTime}' to stream processor '{streamProcessorId}' because of: {reason}");
            return _streamProcessorStates.AddFailingPartition(streamProcessorId, partition, position, failureType, retryTime, reason, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<StreamProcessorState> CatchupFor(StreamProcessorId streamProcessorId, IEventProcessor eventProcessor, StreamProcessorState streamProcessorState, CancellationToken cancellationToken)
        {
            if (streamProcessorState.FailingPartitions.Count > 0) streamProcessorState = await _streamProcessorStates.GetOrAddNew(streamProcessorId, cancellationToken).ConfigureAwait(false);
            var failingPartitionsList = streamProcessorState.FailingPartitions.ToList();
            foreach (var kvp in failingPartitionsList)
            {
                var partition = kvp.Key;
                var failingPartitionState = kvp.Value;
                if (ShouldRetryProcessing(failingPartitionState))
                {
                    var nextPosition = await FindPositionOfNextEventInPartition(streamProcessorId, partition, failingPartitionState.Position, cancellationToken).ConfigureAwait(false);
                    _logger.Debug($"Catching up partition '{partition}' in Stream Processor '{streamProcessorId}' starting at position {nextPosition}");
                    while (ShouldProcessNextEventInPartition(nextPosition, streamProcessorState.Position))
                    {
                        if (!ShouldRetryProcessing(failingPartitionState)) break;

                        var streamEvent = await FetchEventWithPartitionAtPosition(streamProcessorId, nextPosition, cancellationToken).ConfigureAwait(false);
                        var processingResult = await ProcessEvent(failingPartitionState, eventProcessor, streamEvent.Event, partition, cancellationToken).ConfigureAwait(false);

                        if (processingResult.Succeeded)
                        {
                            (streamProcessorState, failingPartitionState) = await ChangePositionInFailingPartition(streamProcessorId, partition, failingPartitionState.Position, nextPosition.Increment(), cancellationToken).ConfigureAwait(false);
                        }
                        else if (processingResult.Retry)
                        {
                            (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(streamProcessorId, partition, processingResult.Failure.Type, failingPartitionState.ProcessingAttempts + 1, processingResult.Failure.RetryTimeout, processingResult.Failure.Reason, nextPosition, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(streamProcessorId, partition, processingResult.Failure.Type, failingPartitionState.ProcessingAttempts + 1, DateTimeOffset.MaxValue, processingResult.Failure.Reason, nextPosition, cancellationToken).ConfigureAwait(false);
                        }

                        nextPosition = await FindPositionOfNextEventInPartition(streamProcessorId, partition, failingPartitionState.Position, cancellationToken).ConfigureAwait(false);
                    }

                    _logger.Debug($"Done catching up partition '{partition}' in Stream Processor '{streamProcessorId}'");
                    if (ShouldRetryProcessing(failingPartitionState)) streamProcessorState = await RemoveFailingPartition(streamProcessorId, partition, cancellationToken).ConfigureAwait(false);
                }
            }

            return streamProcessorState;
        }

        Task<StreamProcessorState> RemoveFailingPartition(StreamProcessorId streamProcessorId, PartitionId partition, CancellationToken cancellationToken)
        {
            _logger.Debug($"Removing failing partition '{partition}' from stream processor '{streamProcessorId}'");
            return _streamProcessorStates.RemoveFailingPartition(streamProcessorId, partition, cancellationToken);
        }

        async Task<(StreamProcessorState, FailingPartitionState)> ChangePositionInFailingPartition(StreamProcessorId streamProcessorId, PartitionId partition, StreamPosition oldPosition, StreamPosition newPosition, CancellationToken cancellationToken)
        {
            _logger.Debug($"Changing position in failting partition {partition} from '{oldPosition}' to '{newPosition}'");
            var newFailingPartitionState = new FailingPartitionState { Position = newPosition, RetryTime = DateTimeOffset.MinValue, ProcessingAttempts = 0 };
            var streamProcessorState = await _streamProcessorStates.SetFailingPartitionState(
                streamProcessorId,
                partition,
                newFailingPartitionState,
                cancellationToken).ConfigureAwait(false);
            return (streamProcessorState, newFailingPartitionState);
        }

        Task<StreamEvent> FetchEventWithPartitionAtPosition(StreamProcessorId streamProcessorId, StreamPosition position, CancellationToken cancellationToken)
        {
            _logger.Debug($"Failing partition in Stream Processor '{streamProcessorId}' is fetching event at position '{position}'.");
            return _eventsFromStreamsFetcher.Fetch(streamProcessorId.ScopeId, streamProcessorId.SourceStreamId, position, cancellationToken);
        }

        Task<StreamPosition> FindPositionOfNextEventInPartition(StreamProcessorId streamProcessorId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken)
        {
            _logger.Debug($"Fetching next event to process in partition '{partitionId}' from position '{fromPosition}'.");
            return _eventsFromStreamsFetcher.FindNext(streamProcessorId.ScopeId, streamProcessorId.SourceStreamId, partitionId, fromPosition, cancellationToken);
        }

        Task<(StreamProcessorState, FailingPartitionState)> SetFailingPartitionState(StreamProcessorId streamProcessorId, PartitionId partitionId, ProcessorFailureType failureType, uint processingAttempts, TimeSpan retryTimeout, string reason, StreamPosition position, CancellationToken cancellationToken) => SetFailingPartitionState(streamProcessorId, partitionId, failureType, processingAttempts, DateTimeOffset.UtcNow.Add(retryTimeout), reason, position, cancellationToken);

        async Task<(StreamProcessorState, FailingPartitionState)> SetFailingPartitionState(StreamProcessorId streamProcessorId, PartitionId partitionId, ProcessorFailureType failureType, uint processingAttempts, DateTimeOffset retryTime, string reason, StreamPosition position, CancellationToken cancellationToken)
        {
            _logger.Debug($" Setting retry time '{retryTime}' and position '{position}' for partition '{partitionId}' in Stream Processor '{streamProcessorId}'");
            var newFailingPartitionState = new FailingPartitionState { Position = position, RetryTime = retryTime, Reason = reason, FailureType = failureType, ProcessingAttempts = processingAttempts };
            var streamProcessorState = await _streamProcessorStates.SetFailingPartitionState(
                streamProcessorId,
                partitionId,
                newFailingPartitionState,
                cancellationToken).ConfigureAwait(false);

            return (streamProcessorState, newFailingPartitionState);
        }

        Task<IProcessingResult> ProcessEvent(FailingPartitionState failingPartitionState, IEventProcessor eventProcessor, CommittedEvent @event, PartitionId partition, CancellationToken cancellationToken)
        {
            _logger.Debug($"Failing partition '{partition}' is processing event '{@event.Type.Id}'");
            var retryProcessingState = failingPartitionState.ProcessingAttempts == 0 ?
                null
                : new grpc.RetryProcessingState
                    {
                        FailureReason = failingPartitionState.Reason,
                        FailureType = (grpc.RetryProcessingState.Types.FailureType)failingPartitionState.FailureType,
                        RetryCount = failingPartitionState.ProcessingAttempts - 1
                    };
            return eventProcessor.Process(@event, partition, retryProcessingState, cancellationToken);
        }

        bool ShouldProcessNextEventInPartition(StreamPosition failingPartitionPosition, StreamPosition streamProcessorPosition) =>
            failingPartitionPosition.Value < streamProcessorPosition.Value;

        bool ShouldRetryProcessing(FailingPartitionState state) =>
            DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
    }
}