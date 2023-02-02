// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned;

/// <summary>
/// Represents an implementation of <see cref="IFailingPartitions" />.
/// </summary>
public class FailingPartitions : IFailingPartitions
{
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly IEventProcessor _eventProcessor;
    readonly ICanFetchEventsFromPartitionedStream _eventsFromStreamsFetcher;
    readonly Func<StreamEvent, ExecutionContext> _createExecutionContextForEvent;
    readonly IEventFetcherPolicies _eventFetcherPolicies;

    /// <summary>
    /// Initializes a new instance of the <see cref="FailingPartitions"/> class.
    /// </summary>
    /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStates" />.</param>
    /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
    /// <param name="eventsFromStreamsFetcher">The <see cref="ICanFetchEventsFromPartitionedStream" />.</param>
    /// <param name="createExecutionContextForEvent">The factory to use to create execution contexts for event processing.</param>
    /// <param name="eventFetcherPolicies">The policies to use for fetching events.</param>
    public FailingPartitions(
        IStreamProcessorStates streamProcessorStates,
        IEventProcessor eventProcessor,
        ICanFetchEventsFromPartitionedStream eventsFromStreamsFetcher,
        Func<StreamEvent, ExecutionContext> createExecutionContextForEvent, //TODO: Oh man, here we go again.
        IEventFetcherPolicies eventFetcherPolicies)
    {
        _streamProcessorStates = streamProcessorStates;
        _eventProcessor = eventProcessor;
        _eventsFromStreamsFetcher = eventsFromStreamsFetcher;
        _createExecutionContextForEvent = createExecutionContextForEvent;
        _eventFetcherPolicies = eventFetcherPolicies;
    }

    /// <inheritdoc/>
    public async Task<IStreamProcessorState> AddFailingPartitionFor(
        IStreamProcessorId streamProcessorId,
        StreamProcessorState oldState,
        ProcessingPosition failedPosition,
        PartitionId partition,
        DateTimeOffset retryTime,
        string reason,
        CancellationToken cancellationToken)
    {
        var failingPartition =
            new FailingPartitionState(failedPosition.StreamPosition, failedPosition.EventLogPosition, retryTime, reason, 1, DateTimeOffset.UtcNow);
        var failingPartitions = new Dictionary<PartitionId, FailingPartitionState>(oldState.FailingPartitions)
        {
            [partition] = failingPartition
        };
        var newState = new StreamProcessorState(failedPosition.StreamPosition + 1, failedPosition.EventLogPosition + 1, failingPartitions,
            oldState.LastSuccessfullyProcessed);
        await PersistNewState(streamProcessorId, newState, cancellationToken).ConfigureAwait(false);
        return newState;
    }

    public Task<IStreamProcessorState> AddFailingPartitionFor(IStreamProcessorId streamProcessorId, StreamProcessorState oldState,
        StreamPosition failedPosition, PartitionId partition,
        DateTimeOffset retryTime, string reason, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task<IStreamProcessorState> CatchupFor(
        IStreamProcessorId streamProcessorId,
        StreamProcessorState streamProcessorState,
        CancellationToken cancellationToken)
    {
        if (streamProcessorState.FailingPartitions.Count > 0)
        {
            streamProcessorState = (await _streamProcessorStates.TryGetFor(streamProcessorId, cancellationToken)
                .ConfigureAwait(false)).Result as StreamProcessorState;
        }

        var failingPartitionsList = streamProcessorState.FailingPartitions.ToList();

        // TODO: Failing partitions should be actorified
        foreach (var kvp in failingPartitionsList)
        {
            var partition = kvp.Key;
            var failingPartitionState = kvp.Value;
            while (ShouldProcessNextEventInPartition(failingPartitionState.Position, streamProcessorState.Position) &&
                   ShouldRetryProcessing(failingPartitionState))
            {
                var tryGetEvents = await _eventFetcherPolicies.Fetching.ExecuteAsync(
                    _ => _eventsFromStreamsFetcher.FetchInPartition(partition, failingPartitionState.Position, _),
                    cancellationToken).ConfigureAwait(false);
                if (!tryGetEvents.Success)
                {
                    break;
                }

                foreach (var streamEvent in tryGetEvents.Result)
                {
                    if (streamEvent.Partition != partition)
                    {
                        throw new StreamEventInWrongPartition(streamEvent, partition);
                    }

                    if (!ShouldProcessNextEventInPartition(streamEvent.Position, streamProcessorState.Position))
                    {
                        break;
                    }

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
                        (streamProcessorState, failingPartitionState) = await ChangePositionInFailingPartition(
                            streamProcessorId,
                            streamProcessorState,
                            partition,
                            new ProcessingPosition(streamEvent.Position + 1, streamEvent.Event.EventLogSequenceNumber + 1),
                            failingPartitionState.LastFailed,
                            cancellationToken).ConfigureAwait(false);
                    }
                    else if (processingResult.Retry)
                    {
                        (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(
                            streamProcessorId,
                            streamProcessorState,
                            partition,
                            failingPartitionState.ProcessingAttempts + 1,
                            processingResult.RetryTimeout,
                            processingResult.FailureReason,
                            new ProcessingPosition(streamEvent.Position, streamEvent.Event.EventLogSequenceNumber),
                            DateTimeOffset.UtcNow,
                            cancellationToken).ConfigureAwait(false);
                        // Important to not process the next events if this failed
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
                            new ProcessingPosition(streamEvent.Position, streamEvent.Event.EventLogSequenceNumber),
                            DateTimeOffset.UtcNow,
                            cancellationToken).ConfigureAwait(false);
                        // Important to not process the next events if this failed
                        break;
                    }
                }
            }

            if (ShouldRetryProcessing(failingPartitionState))
            {
                streamProcessorState =
                    await RemoveFailingPartition(streamProcessorId, streamProcessorState, partition, cancellationToken).ConfigureAwait(false);
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

        var newState = position.StreamPosition > oldState.FailingPartitions[partitionId].Position
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

    static bool ShouldProcessNextEventInPartition(StreamPosition failingPartitionPosition, StreamPosition streamProcessorPosition) =>
        failingPartitionPosition.Value < streamProcessorPosition.Value;

    static bool ShouldRetryProcessing(FailingPartitionState state) =>
        DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
}