// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
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

public class PartitionedProcessor : ProcessorBase<StreamProcessorState>
{
    record State(StreamProcessorState ProcessorState, ImmutableDictionary<PartitionId, ImmutableList<StreamEvent>> CatchUpEvents)
    {
        [Pure]
        public State AddCatchUpEvent(StreamEvent evt)
        {
            return this with
            {
                CatchUpEvents = CatchUpEvents.SetItem(evt.Partition,
                    CatchUpEvents.TryGetValue(evt.Partition, out var list) ? list.Add(evt) : ImmutableList.Create(evt))
            };
        }

        [Pure]
        public State WithoutPreviousCatchUpEvents(StreamEvent evt)
        {
            if (CatchUpEvents.TryGetValue(evt.Partition, out var list))
            {
                var index = list.FindIndex(e => ReferenceEquals(e, evt));
                if (index == list.Count - 1)
                {
                    return RemoveCatchUpPartition(evt.Partition);
                }

                if (index > 0)
                {
                    return this with
                    {
                        CatchUpEvents = CatchUpEvents.SetItem(evt.Partition, list.RemoveRange(0, index))
                    };
                }

                return this;
            }

            // No events for this partition
            return this;
        }

        [Pure]
        public State RemoveCatchUpPartition(PartitionId partition)
        {
            return this with
            {
                CatchUpEvents = CatchUpEvents.Remove(partition)
            };
        }

        public bool HasFailedCatchUpEvents => CatchUpEvents.Any();

        public State ClearPartitionsWithoutCatchUpEvents()
        {
            var partitionsToClear = ProcessorState.FailingPartitions.Keys.Except(CatchUpEvents.Keys).ToHashSet();
            return this with
            {
                ProcessorState = ProcessorState.WithoutFailingPartitions(partitionsToClear)
            };
        }
    }

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
        :
        base(
            streamProcessorId, processor, streamProcessorStates, executionContext, onProcessed, onFailedToProcess, tenantId, logger)
    {
    }

    public async Task Process(ChannelReader<StreamEvent> messages, IStreamProcessorState state, CancellationToken cancellationToken)
    {
        try
        {
            var currentState = new State(AsPartitioned(state), ImmutableDictionary<PartitionId, ImmutableList<StreamEvent>>.Empty);

            while (!cancellationToken.IsCancellationRequested)
            {
                var nextAction = await WaitForNextAction(messages, currentState, cancellationToken);

                try
                {
                    switch (nextAction)
                    {
                        case NextAction.ProcessCatchUpEvent:
                            var CatchUpEvent = await messages.ReadAsync(cancellationToken);
                            // Logger.LogInformation("Processing CatchUp event {Partition}:{ProcessingPosition}", CatchUpEvent.Partition,
                            //     CatchUpEvent.CurrentProcessingPosition);
                            currentState = await HandleCatchUpEvent(CatchUpEvent, currentState, cancellationToken);
                            break;
                        case NextAction.ProcessNextEvent:
                            var evt = await messages.ReadAsync(cancellationToken);
                            // Logger.LogInformation("Processing next event {Partition}:{ProcessingPosition}", evt.Partition, evt.CurrentProcessingPosition);
                            currentState = await HandleNewEvent(evt, currentState, cancellationToken);
                            break;
                        case NextAction.ProcessFailedEvents:
                            currentState = await CatchUpFor(currentState, cancellationToken);
                            break;
                    }
                }
                finally
                {
                    await PersistNewState(currentState.ProcessorState, CancellationToken.None);
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
        ProcessCatchUpEvent,

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
    async ValueTask<NextAction> WaitForNextAction(ChannelReader<StreamEvent> messages, State state, CancellationToken cancellationToken)
    {
        if (!state.HasFailedCatchUpEvents || !TryGetTimeToRetry(state, out var timeToRetry))
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
                    if (evt.Event.EventLogSequenceNumber < state.ProcessorState.Position.EventLogPosition)
                    {
                        return NextAction.ProcessCatchUpEvent;
                    }

                    return NextAction.ProcessNextEvent;
                }

                return NextAction.ProcessNextEvent;
            }

            throw new OperationCanceledException("Channel was closed");
        }
    }

    bool TryGetTimeToRetry(State state, out TimeSpan timeToRetry)
    {
        timeToRetry = TimeSpan.MaxValue;
        foreach (var (partitionId, failingPartitionState) in state.ProcessorState.FailingPartitions)
        {
            if (!state.CatchUpEvents.ContainsKey(partitionId)) continue;
            if (failingPartitionState.TryGetTimespanToRetry(out var partitionTimeToRetry) && partitionTimeToRetry < timeToRetry)
            {
                timeToRetry = partitionTimeToRetry;
            }
        }

        return timeToRetry < TimeSpan.MaxValue;
    }

    async Task<State> HandleCatchUpEvent(StreamEvent evt, State existingState, CancellationToken cancellationToken)
    {
        if (existingState.ProcessorState.FailingPartitions.TryGetValue(evt.Partition, out var failingPartition))
        {
            return await HandleCatchUpEventForPartition(evt, existingState, failingPartition, cancellationToken);
        }

        // Already processed, ignore
        Logger.LogInformation("Already processed event {Position}, currently at {CurrentPosition}", evt.CurrentProcessingPosition,
            existingState.ProcessorState.Position);
        return existingState;
    }

    async Task<State> HandleNewEvent(StreamEvent evt, State state, CancellationToken cancellationToken)
    {
        if (_catchingUp) // Finished catching up, clear all partitions with failed events that do not have any more events to process
        {
            _catchingUp = false;
            state = state.ClearPartitionsWithoutCatchUpEvents();
            await PersistNewState(newState: state.ProcessorState, cancellationToken);
        }

        if (state.ProcessorState.FailingPartitions.TryGetValue(evt.Partition, out var failingPartitionState))
        {
            if (failingPartitionState.CanBeRetried) //Ignore events from partitions that are not retryable
            {
                state = state.AddCatchUpEvent(evt);
            }

            return state with
            {
                ProcessorState = state.ProcessorState.WithResult(SkippedProcessing.Instance, evt, DateTimeOffset.UtcNow)
            };
        }

        var (processorState, processingResult) = await ProcessEvent(evt, state.ProcessorState, GetExecutionContextForEvent(evt), cancellationToken);
        state = state with
        {
            ProcessorState = processorState
        };
        if (processingResult is { Succeeded: false, Retry: true })
        {
            state = state.AddCatchUpEvent(evt);
        }

        return state;
    }


    async Task<State> HandleCatchUpEventForPartition(StreamEvent evt, State state, FailingPartitionState failingPartitionState,
        CancellationToken cancellationToken)
    {
        if (!failingPartitionState.CanBeRetried)
        {
            // Ignore
            return state;
        }

        if (state.CatchUpEvents.ContainsKey(evt.Partition))
        {
            // Currently waiting for retry, events are already written to the failed partition
            return state.AddCatchUpEvent(evt);
        }

        if (failingPartitionState.TryGetTimespanToRetry(out var timeToRetry) && timeToRetry > TimeSpan.Zero)
        {
            // Currently waiting for retry
            return state.AddCatchUpEvent(evt);
        }

        // Retry immediately
        var (newState, processingResult) = await RetryProcessingEvent(
            evt,
            state.ProcessorState,
            failingPartitionState.Reason,
            failingPartitionState.ProcessingAttempts,
            GetExecutionContextForEvent(evt),
            cancellationToken).ConfigureAwait(false);
        state = state with { ProcessorState = newState };
        if (processingResult is { Succeeded: false, Retry: true })
        {
            return state.AddCatchUpEvent(evt);
        }

        return state;
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


    async Task<State> CatchUpFor(
        State state,
        CancellationToken cancellationToken)
    {
        foreach (var (partition, events) in state.CatchUpEvents)
        {
            state = await CatchUpPartition(state, partition, events, cancellationToken);
        }

        return state;
    }

    async Task<State> CatchUpPartition(
        State state,
        PartitionId partition,
        IReadOnlyList<StreamEvent> events,
        CancellationToken cancellationToken)
    {
        var failingPartitionState = state.ProcessorState.FailingPartitions[partition];
        if (!ShouldRetryProcessing(failingPartitionState)) return state;

        for (var index = 0; index < events.Count; index++)
        {
            var streamEvent = events[index];

            var (newState, processingResult) = await RetryProcessingEvent(
                streamEvent,
                state.ProcessorState,
                failingPartitionState.Reason,
                failingPartitionState.ProcessingAttempts,
                GetExecutionContextForEvent(streamEvent),
                cancellationToken).ConfigureAwait(false);
            await PersistNewState(newState, CancellationToken.None);

            state = state with
            {
                ProcessorState = newState
            };

            if (processingResult.Succeeded)
            {
                continue;
            }

            // Failed, retry later
            return state.WithoutPreviousCatchUpEvents(streamEvent);
        }

        // All current events processed.
        state = state.RemoveCatchUpPartition(partition);

        if (!_catchingUp) // Since we are current with the stream, we can remove the failing partition
        {
            state = state with { ProcessorState = state.ProcessorState.WithoutFailingPartition(partition) };
            await PersistNewState(state.ProcessorState, CancellationToken.None);
        }

        return state;
    }

    static bool ShouldRetryProcessing(FailingPartitionState state) =>
        DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
}
