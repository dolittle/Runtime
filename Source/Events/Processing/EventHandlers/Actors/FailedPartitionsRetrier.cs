// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

/// <summary>
/// Represents an implementation of <see cref="IHandleFailures" />.
/// </summary>
public class FailedPartitionsRetrier
{
    readonly Dictionary<PartitionId, List<StreamEvent>> _failedEvents = new();

    readonly IStreamProcessorStates _streamProcessorStates;

    readonly IEventProcessor _eventProcessor;

    readonly Func<StreamEvent, ExecutionContext> _createExecutionContextForEvent;

    /// <summary>
    /// Initializes a new instance of the <see cref="FailedPartitionsRetrier"/> class.
    /// </summary>
    /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStates" />.</param>
    /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
    /// <param name="createExecutionContextForEvent">The factory to use to create execution contexts for event processing.</param>
    public FailedPartitionsRetrier(
        IStreamProcessorStates streamProcessorStates,
        IEventProcessor eventProcessor,
        Func<StreamEvent, ExecutionContext> createExecutionContextForEvent)
    {
        _streamProcessorStates = streamProcessorStates;
        _eventProcessor = eventProcessor;
        _createExecutionContextForEvent = createExecutionContextForEvent;
    }

    public void WriteToFailedPartition(StreamEvent evt)
    {
        if (!_failedEvents.TryGetValue(evt.Partition, out var list))
        {
            list = new List<StreamEvent>();
            _failedEvents.Add(evt.Partition, list);
        }

        list.Add(evt);
    }
    
    public bool HasFailedEvents => _failedEvents.Any();


    /// <inheritdoc/>
    public async Task<StreamProcessorState> CatchupFor(
        IStreamProcessorId streamProcessorId,
        StreamProcessorState streamProcessorState,
        CancellationToken cancellationToken)
    {
        foreach (var (partition, events) in _failedEvents)
        {
            streamProcessorState = await CatchupPartition(streamProcessorId, streamProcessorState, cancellationToken, partition, events);
        }

        return streamProcessorState;
    }

    async Task<StreamProcessorState> CatchupPartition(IStreamProcessorId streamProcessorId, StreamProcessorState streamProcessorState,
        CancellationToken cancellationToken,
        PartitionId partition, IReadOnlyList<StreamEvent> events)
    {
        var failingPartitionState = streamProcessorState.FailingPartitions[partition];
        if (!ShouldRetryProcessing(failingPartitionState)) return streamProcessorState;
        for (var index = 0; index < events.Count; index++)
        {
            var nextIndex = index + 1;
            var streamEvent = events[index];
            if (!ShouldRetryProcessing(failingPartitionState))
            {
                break;
            }

            var processingResult = await RetryProcessingEvent(
                failingPartitionState,
                streamEvent.Event,
                partition,
                _createExecutionContextForEvent(streamEvent),
                cancellationToken).ConfigureAwait(false);

            if (processingResult.Succeeded)
            {
                if (nextIndex < events.Count)
                {
                    var nextEvent = events[nextIndex];
                    (streamProcessorState, failingPartitionState) = await ChangePositionInFailingPartition(
                        streamProcessorId,
                        streamProcessorState,
                        partition,
                        nextEvent.CurrentProcessingPosition,
                        failingPartitionState.LastFailed,
                        cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    // We are at the end of the list, so we can remove the partition from the failed partitions
                    _failedEvents.Remove(partition);
                    streamProcessorState = await RemoveFailingPartition(streamProcessorId, streamProcessorState, partition, cancellationToken)
                        .ConfigureAwait(false);
                    return streamProcessorState;
                }
            }
            else // Failed
            {

                if (processingResult.Retry)
                {
                    (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(
                        streamProcessorId,
                        streamProcessorState,
                        partition,
                        failingPartitionState.ProcessingAttempts + 1,
                        processingResult.RetryTimeout,
                        processingResult.FailureReason,
                        streamEvent.CurrentProcessingPosition,
                        DateTimeOffset.UtcNow,
                        cancellationToken).ConfigureAwait(false);
                    // Important to not process the next events if this failed
                    _failedEvents[partition] = events.Skip(nextIndex).ToList();
                    break;
                }
                else
                {
                    (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(
                        streamProcessorId,
                        streamProcessorState,
                        partition,
                        failingPartitionState.ProcessingAttempts + 1,
                        DateTimeOffset.MaxValue,
                        processingResult.FailureReason,
                        streamEvent.CurrentProcessingPosition,
                        DateTimeOffset.UtcNow,
                        cancellationToken).ConfigureAwait(false);
                    // Important to not process the next events if this failed
                    _failedEvents[partition] = events.Skip(nextIndex).ToList();

                    break;
                }
            }
        }


        return streamProcessorState;
    }

    async Task<StreamProcessorState> RemoveFailingPartition(IStreamProcessorId streamProcessorId, StreamProcessorState oldState, PartitionId partition,
        CancellationToken cancellationToken)
    {
        var newFailingPartitions = oldState.FailingPartitions;
        newFailingPartitions.Remove(partition);
        var newState = oldState with { FailingPartitions = newFailingPartitions };
        oldState.FailingPartitions.Remove(partition);

        await PersistNewState(streamProcessorId, newState, cancellationToken).ConfigureAwait(false);
        return newState;
    }

    Task<(StreamProcessorState, FailingPartitionState)> ChangePositionInFailingPartition(
        IStreamProcessorId streamProcessorId,
        StreamProcessorState oldState,
        PartitionId partitionId,
        ProcessingPosition newPosition,
        DateTimeOffset lastFailed,
        CancellationToken cancellationToken) =>
        SetFailingPartitionState(streamProcessorId, oldState, partitionId, 0, DateTimeOffset.UtcNow, string.Empty, newPosition, lastFailed, cancellationToken);

    Task<(StreamProcessorState, FailingPartitionState)> SetFailingPartitionState(
        IStreamProcessorId streamProcessorId,
        StreamProcessorState oldState,
        PartitionId partitionId,
        uint processingAttempts,
        TimeSpan retryTimeout,
        string reason,
        ProcessingPosition position,
        DateTimeOffset lastFailed,
        CancellationToken cancellationToken) =>
        SetFailingPartitionState(streamProcessorId, oldState, partitionId, processingAttempts, DateTimeOffset.UtcNow.Add(retryTimeout), reason, position,
            lastFailed, cancellationToken);

    async Task<(StreamProcessorState, FailingPartitionState)> SetFailingPartitionState(
        IStreamProcessorId streamProcessorId,
        StreamProcessorState oldState,
        PartitionId partitionId,
        uint processingAttempts,
        DateTimeOffset retryTime,
        string reason,
        ProcessingPosition position,
        DateTimeOffset lastFailed,
        CancellationToken cancellationToken)
    {
        var newFailingPartitionState =
            new FailingPartitionState(position.StreamPosition, position.EventLogPosition, retryTime, reason, processingAttempts, lastFailed);
        var newFailingPartitions = oldState.FailingPartitions;
        newFailingPartitions[partitionId] = newFailingPartitionState;

        var newState = position.StreamPosition > oldState.FailingPartitions[partitionId].Position.StreamPosition
            ? oldState with { FailingPartitions = newFailingPartitions, LastSuccessfullyProcessed = DateTimeOffset.UtcNow }
            : oldState with { FailingPartitions = newFailingPartitions };

        await PersistNewState(streamProcessorId, newState, cancellationToken).ConfigureAwait(false);

        return (newState, newFailingPartitionState);
    }

    Task<IProcessingResult> RetryProcessingEvent(FailingPartitionState failingPartitionState, CommittedEvent @event, PartitionId partition,
        ExecutionContext executionContext, CancellationToken cancellationToken) =>
        _eventProcessor.Process(@event, partition, failingPartitionState.Reason,
            failingPartitionState.ProcessingAttempts == 0 ? 0 : failingPartitionState.ProcessingAttempts - 1, executionContext, cancellationToken);

    Task PersistNewState(IStreamProcessorId streamProcessorId, StreamProcessorState newState, CancellationToken cancellationToken) =>
        _streamProcessorStates.Persist(streamProcessorId, newState, cancellationToken);

    static bool ShouldProcessNextEventInPartition(ProcessingPosition failingPartitionPosition, ProcessingPosition streamProcessorPosition) =>
        failingPartitionPosition.StreamPosition.Value < streamProcessorPosition.StreamPosition.Value;

    static bool ShouldRetryProcessing(FailingPartitionState state) =>
        DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
}
