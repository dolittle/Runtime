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
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

public class PartitionedProcessor : ProcessorBase
{
    readonly Dictionary<PartitionId, List<StreamEvent>> _failedEvents = new();
    bool _catchingUp = true;

    public PartitionedProcessor(
        StreamProcessorId streamProcessorId,
        IEventProcessor processor,
        IStreamProcessorStates streamProcessorStates,
        ExecutionContext executionContext,
        ScopedStreamProcessorProcessedEvent onProcessed,
        ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
        TenantId tenantId,
        ILogger logger)
        : base(
            streamProcessorId, processor, streamProcessorStates, executionContext, onProcessed, onFailedToProcess, tenantId, logger)
    {
    }

    public async Task Process(ChannelReader<StreamEvent> messages, IStreamProcessorState currentState, CancellationToken cancellationToken)
    {
        try
        {
            var partitionedState = AsPartitioned(currentState); // Verify that the state is of correct type

            while (!cancellationToken.IsCancellationRequested)
            {
                var nextAction = await WaitForNextAction(messages, AsPartitioned(currentState), cancellationToken);

                switch (nextAction)
                {
                    case NextAction.ProcessCatchupEvent:
                        var catchupEvent = await messages.ReadAsync(cancellationToken);
                        partitionedState = await HandleCatchupEvent(catchupEvent, partitionedState, cancellationToken);
                        break;
                    case NextAction.ProcessNextEvent:
                        var evt = await messages.ReadAsync(cancellationToken);
                        partitionedState = await HandleNewEvent(evt, partitionedState, cancellationToken);
                        break;
                    case NextAction.ProcessFailedEvents:
                        currentState = await CatchupFor(partitionedState, cancellationToken);
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
    }

    enum NextAction
    {
        /// <summary>
        /// Process the next event in the event stream.
        /// </summary>
        ProcessNextEvent,

        /// <summary>
        /// Process an event from before the current position in the event stream.
        /// Skips events in non failing partitions
        /// </summary>
        ProcessCatchupEvent,

        /// <summary>
        /// Retry processing of failed events.
        /// </summary>
        ProcessFailedEvents,
    }

    /// <summary>
    /// Determines which action to take next.
    /// Either waits for new data to become available or for the retry delay to expire.
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    async ValueTask<NextAction> WaitForNextAction(ChannelReader<StreamEvent> messages, StreamProcessorState state, CancellationToken cancellationToken)
    {
        if (!HasFailedEvents || !TryGetTimeToRetry(state, out var timeToRetry))
        {
            return await WaitForNextEvent();
        }

        if (timeToRetry <= TimeSpan.Zero)
        {
            return NextAction.ProcessFailedEvents;
        }

        var retryFailureAfter = Task.Delay(timeToRetry, cancellationToken);
        var readyToRead = messages.WaitToReadAsync(cancellationToken).AsTask();
        await Task.WhenAny(retryFailureAfter, readyToRead);
        if (retryFailureAfter.IsCompletedSuccessfully)
        {
            return NextAction.ProcessFailedEvents;
        }

        return await WaitForNextEvent();

        async Task<NextAction> WaitForNextEvent()
        {
            var notClosed = await messages.WaitToReadAsync(cancellationToken);
            if (notClosed)
            {
                if (_catchingUp && messages.TryPeek(out var evt))
                {
                    if (evt.Event.EventLogSequenceNumber < state.Position.EventLogPosition)
                    {
                        return NextAction.ProcessCatchupEvent;
                    }

                    return NextAction.ProcessNextEvent;
                }

                return NextAction.ProcessNextEvent;
            }

            throw new OperationCanceledException("Channel was closed");
        }
    }

    bool TryGetTimeToRetry(StreamProcessorState streamProcessorState, out TimeSpan timeToRetry)
    {
        timeToRetry = TimeSpan.MaxValue;
        foreach (var (partitionId, failingPartitionState) in streamProcessorState.FailingPartitions)
        {
            if (!_failedEvents.ContainsKey(partitionId)) continue;
            if (failingPartitionState.TryGetTimespanToRetry(out var partitionTimeToRetry) && partitionTimeToRetry < timeToRetry)
            {
                timeToRetry = partitionTimeToRetry;
            }
        }

        return timeToRetry < TimeSpan.MaxValue;
    }

    async Task<StreamProcessorState> HandleCatchupEvent(StreamEvent evt, StreamProcessorState existingState, CancellationToken cancellationToken)
    {
        if (existingState.FailingPartitions.TryGetValue(evt.Partition, out var failingPartition))
        {
            return await HandleCatchupEventForPartition(evt, failingPartition, existingState, cancellationToken);
        }

        // Already processed, ignore
        Logger.LogInformation("Already processed event {Position}, currently at {CurrentPosition}", evt.CurrentProcessingPosition, existingState.Position);
        return existingState;
    }

    async Task<StreamProcessorState> HandleNewEvent(StreamEvent evt, StreamProcessorState existingState, CancellationToken cancellationToken)
    {
        if (_catchingUp) // Finished catching up, clear all partitions with failed events that do not have any more events to process
        {
            _catchingUp = false;

            var noLongerFailingPartitions = existingState.FailingPartitions.Where(_ => !_failedEvents.ContainsKey(_.Key)).Select(_ => _.Key).ToArray();
            existingState = existingState.WithoutFailingPartitions(noLongerFailingPartitions);
            await PersistNewState(Identifier, newState: existingState, cancellationToken);
        }

        if (existingState.FailingPartitions.TryGetValue(evt.Partition, out var failingPartitionState))
        {
            if (failingPartitionState.CanBeRetried) //Ignore events from partitions that are not retryable
            {
                WriteToFailedPartition(evt);
            }

            return existingState;
        }

        var (state, processingResult) = await ProcessEvent(evt, existingState, GetExecutionContextForEvent(evt), cancellationToken);
        if (processingResult is { Succeeded: false, Retry: true })
        {
            WriteToFailedPartition(evt);
        }

        return AsPartitioned(state);
    }


    async Task<StreamProcessorState> HandleCatchupEventForPartition(StreamEvent evt, FailingPartitionState failingPartitionState,
        StreamProcessorState existingState,
        CancellationToken cancellationToken)
    {
        if (!failingPartitionState.CanBeRetried)
        {
            // Ignore
            return existingState;
        }

        if (_failedEvents.ContainsKey(evt.Partition))
        {
            // Currently waiting for retry, events are already written to the failed partition
            WriteToFailedPartition(evt);
            return existingState;
        }

        if (failingPartitionState.TryGetTimespanToRetry(out var timeToRetry) && timeToRetry > TimeSpan.Zero)
        {
            // Currently waiting for retry
            WriteToFailedPartition(evt);
            return existingState;
        }

        // Retry immediately
        var (newState, processingResult) = await RetryProcessingEvent(
            evt,
            existingState,
            failingPartitionState.Reason,
            failingPartitionState.ProcessingAttempts,
            GetExecutionContextForEvent(evt),
            cancellationToken).ConfigureAwait(false);

        if (processingResult is { Succeeded: false, Retry: true })
        {
            WriteToFailedPartition(evt);
        }

        return AsPartitioned(newState);
    }

    StreamProcessorState AsPartitioned(IStreamProcessorState state)
    {
        switch (state)
        {
            case StreamProcessorState partitionedState:
                return partitionedState;

            case Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState nonPartitionedState:
                if (!nonPartitionedState.IsFailing)
                {
                    Logger.LogInformation("Converting non-partitioned state to partitioned for {StreamProcessorId}", Identifier);
                    return new StreamProcessorState(nonPartitionedState.Position, nonPartitionedState.LastSuccessfullyProcessed);
                }

                throw new ArgumentException("State is not convertible to partitioned");

            default:
                throw new ArgumentException("State is of invalid type");
        }
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
        StreamProcessorState streamProcessorState,
        PartitionId partition,
        IReadOnlyList<StreamEvent> events,
        CancellationToken cancellationToken)
    {
        var failingPartitionState = streamProcessorState.FailingPartitions[partition];
        if (!ShouldRetryProcessing(failingPartitionState)) return streamProcessorState;

        for (var index = 0; index < events.Count; index++)
        {
            var streamEvent = events[index];

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
                continue;
            }

            // Failed, retry later
            _failedEvents[partition] = events.Skip(index).ToList();
            return AsPartitioned(newState);

            // Failed
        }

        // All current events processed.
        _failedEvents.Remove(partition);
        if (!_catchingUp)
        {
            streamProcessorState = streamProcessorState.WithoutFailingPartitions(partition);
        }

        return streamProcessorState;
    }

    static bool ShouldRetryProcessing(FailingPartitionState state) =>
        DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
}
