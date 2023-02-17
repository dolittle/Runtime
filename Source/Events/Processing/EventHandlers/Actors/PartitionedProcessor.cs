// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Proto;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

public class PartitionedProcessor : ProcessorBase
{
    readonly Dictionary<PartitionId, List<StreamEvent>> _failedEvents = new();


    public PartitionedProcessor(
        StreamProcessorId streamProcessorId,
        TypeFilterWithEventSourcePartitionDefinition filterDefinition,
        IEventProcessor processor,
        IStreamProcessorStates streamProcessorStates,
        ExecutionContext executionContext,
        ScopedStreamProcessorProcessedEvent onProcessed,
        ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
        TenantId tenantId,
        ILogger logger)
        : base(
            streamProcessorId, filterDefinition, processor, streamProcessorStates, executionContext, onProcessed, onFailedToProcess, tenantId, logger)
    {
    }


    public async Task Process(ChannelReader<StreamEvent> messages, IStreamProcessorState currentState, IContext ctx)
    {
        try
        {
            var partitionedState = AsPartitioned(currentState); // Verify that the state is of correct type

            while (!ctx.CancellationToken.IsCancellationRequested)
            {
                var nextAction = await WaitForNextAction(messages, currentState);

                switch (nextAction)
                {
                    case NextAction.ProcessNextEvent:
                        var evt = await messages.ReadAsync(ctx.CancellationToken);
                        partitionedState = await HandleNewEvent(evt, partitionedState, ctx);
                        break;
                    case NextAction.ProcessFailedEvents:
                        currentState = await CatchupFor(partitionedState, ctx.CancellationToken);
                        break;
                }
            }
        }
        catch (OperationCanceledException e)
        {
            Logger.CancelledRunningEventHandler(e, Identifier.EventProcessorId, Identifier.ScopeId);
        }
        catch (Exception e)
        {
            Logger.ErrorWhileRunningEventHandler(e, Identifier.EventProcessorId, Identifier.ScopeId);
        }
        finally
        {
            ctx.Stop(ctx.Self);
        }
    }

    enum NextAction
    {
        ProcessNextEvent,
        ProcessFailedEvents,
    }

    /// <summary>
    /// Determines which action to take next.
    /// Either waits for new data to become available or for the retry delay to expire.
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    async ValueTask<NextAction> WaitForNextAction(ChannelReader<StreamEvent> messages, IStreamProcessorState state)
    {
        if (!HasFailedEvents || state.TryGetTimespanToRetry(out var timeToRetry))
        {
            return await WaitForNextEvent();
        }


        if (timeToRetry <= TimeSpan.Zero)
        {
            return NextAction.ProcessFailedEvents;
        }

        var retryFailureAfter = Task.Delay(timeToRetry);
        var readyToRead = messages.WaitToReadAsync().AsTask();
        await Task.WhenAny(retryFailureAfter, readyToRead);
        if (retryFailureAfter.IsCompletedSuccessfully)
        {
            return NextAction.ProcessFailedEvents;
        }

        return await WaitForNextEvent();

        async Task<NextAction> WaitForNextEvent()
        {
            var notClosed = await messages.WaitToReadAsync();
            if (notClosed)
            {
                return NextAction.ProcessNextEvent;
            }

            throw new OperationCanceledException("Channel was closed");
        }
    }


    async Task<StreamProcessorState> HandleNewEvent(StreamEvent evt, StreamProcessorState existingState, IContext ctx)
    {
        if (existingState.FailingPartitions.TryGetValue(evt.Partition, out var failingPartitionState))
        {
            if (evt.Event.EventLogSequenceNumber >= failingPartitionState.Position.EventLogPosition)
            {
                WriteToFailedPartition(evt);
            }

            return existingState;
        }

        if (evt.Event.EventLogSequenceNumber < existingState.Position.EventLogPosition)
        {
            // Already processed, ignore
            Logger.LogInformation("Already processed event {Position}, currently at {CurrentPosition}", evt.CurrentProcessingPosition, existingState.Position);
            return existingState;
        }
        

        var (state, processingResult) = await ProcessEvent(evt, existingState, GetExecutionContextForEvent(evt), ctx.CancellationToken);
        if (!processingResult.Succeeded)
        {
            WriteToFailedPartition(evt);
        }

        return AsPartitioned(state);
    }

    static StreamProcessorState AsPartitioned(IStreamProcessorState state)
    {
        if (state is not StreamProcessorState partitionedState)
            throw new ArgumentException("State is not a partitioned state");

        return partitionedState;
    }

    void WriteToFailedPartition(StreamEvent evt)
    {
        if (!_failedEvents.TryGetValue(evt.Partition, out var list))
        {
            list = new List<StreamEvent>();
            _failedEvents.Add(evt.Partition, list);
        }

        list.Add(evt);
    }

    bool HasFailedEvents => _failedEvents.Any();


    async Task<StreamProcessorState> CatchupFor(
        StreamProcessorState streamProcessorState,
        CancellationToken cancellationToken)
    {
        foreach (var (partition, events) in _failedEvents)
        {
            streamProcessorState = await CatchupPartition(streamProcessorState, partition, events, cancellationToken);
        }

        return streamProcessorState;
    }

    async Task<StreamProcessorState> CatchupPartition(
        StreamProcessorState existingStreamProcessorState,
        PartitionId partition,
        IReadOnlyList<StreamEvent> events,
        CancellationToken cancellationToken)
    {
        var failingPartitionState = existingStreamProcessorState.FailingPartitions[partition];
        if (!ShouldRetryProcessing(failingPartitionState)) return existingStreamProcessorState;
        var streamProcessorState = existingStreamProcessorState;
        for (var index = 0; index < events.Count; index++)
        {
            var nextIndex = index + 1;
            var streamEvent = events[index];
            if (!ShouldRetryProcessing(failingPartitionState))
            {
                break;
            }

            var (newState, processingResult) = await RetryProcessingEvent(
                streamEvent,
                streamProcessorState,
                failingPartitionState.Reason,
                failingPartitionState.ProcessingAttempts,
                GetExecutionContextForEvent(streamEvent),
                cancellationToken).ConfigureAwait(false);
            streamProcessorState = AsPartitioned(newState);

            if (processingResult.Succeeded)
            {
                if (nextIndex < events.Count)
                {
                    var nextEvent = events[nextIndex];
                    (streamProcessorState, failingPartitionState) = await ChangePositionInFailingPartition(
                        Identifier,
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
                    streamProcessorState = await RemoveFailingPartition(Identifier, streamProcessorState, partition, cancellationToken)
                        .ConfigureAwait(false);
                    return streamProcessorState;
                }
            }
            else // Failed
            {
                if (processingResult.Retry)
                {
                    (streamProcessorState, failingPartitionState) = await SetFailingPartitionState(
                        Identifier,
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
                        Identifier,
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

    async Task<StreamProcessorState> RemoveFailingPartition(IStreamProcessorId streamProcessorId,
        StreamProcessorState oldState, PartitionId partition,
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

    static bool ShouldRetryProcessing(FailingPartitionState state) =>
        DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
}
