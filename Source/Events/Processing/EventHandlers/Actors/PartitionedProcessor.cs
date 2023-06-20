// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
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
    readonly ICanFetchEventsFromPartitionedStream _fetcher;

    record State(StreamProcessorState ProcessorState)
    {
        public bool NoFailingEvents => ProcessorState.FailingPartitions.IsEmpty;

        public bool TryGetTimeToRetry(out TimeSpan timeToRetry, [NotNullWhen(true)] out PartitionId? selectedPartitionId)
        {
            timeToRetry = TimeSpan.MaxValue;
            selectedPartitionId = default;
            if (NoFailingEvents) return false;

            foreach (var (partitionId, failingPartitionState) in ProcessorState.FailingPartitions)
            {
                if (failingPartitionState.TryGetTimespanToRetry(out var partitionTimeToRetry) && partitionTimeToRetry < timeToRetry)
                {
                    timeToRetry = partitionTimeToRetry;
                    selectedPartitionId = partitionId;
                }
            }

            return timeToRetry < TimeSpan.MaxValue;
        }
    }

    bool _catchingUp = true;
    readonly ImmutableHashSet<Guid> _handledTypes;


    public PartitionedProcessor(
        StreamProcessorId streamProcessorId,
        IEnumerable<ArtifactId> handledEventTypes,
        IEventProcessor processor,
        IStreamProcessorStates streamProcessorStates,
        ExecutionContext executionContext,
        ScopedStreamProcessorProcessedEvent onProcessed,
        ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
        TenantId tenantId,
        ICanFetchEventsFromPartitionedStream fetcher,
        ILogger logger)
        :
        base(
            streamProcessorId, processor, streamProcessorStates, executionContext, onProcessed, onFailedToProcess, tenantId, logger)
    {
        _fetcher = fetcher;
        _handledTypes = handledEventTypes.Select(_ => _.Value).ToImmutableHashSet();
    }

    public async Task Process(ChannelReader<StreamEvent> messages, IStreamProcessorState state, CancellationToken cancellationToken, CancellationToken deadlineToken)
    {
        var currentState = new State(AsPartitioned(state));

        while (!cancellationToken.IsCancellationRequested)
        {
            var (nextAction, partitionId) = await WaitForNextAction(messages, currentState, cancellationToken);

            try
            {
                switch (nextAction)
                {
                    case NextAction.ProcessCatchUpEvent:
                        _ = await messages.ReadAsync(cancellationToken);
                        // Skip message, handled in the catch-up process
                        break;
                    case NextAction.ProcessNextEvent:
                        var evt = await messages.ReadAsync(cancellationToken);
                        currentState = await HandleNewEvent(evt, currentState, deadlineToken);
                        break;
                    case NextAction.ProcessFailedEvents:
                        currentState = await CatchUpForPartition(currentState, partitionId!, cancellationToken,deadlineToken);
                        break;
                }
            }
            finally
            {
                await PersistNewState(currentState.ProcessorState, deadlineToken);
            }
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
    async ValueTask<(NextAction, PartitionId?)> WaitForNextAction(ChannelReader<StreamEvent> messages, State state, CancellationToken cancellationToken)
    {
        if (!state.TryGetTimeToRetry(out var timeToRetry, out var partitionId))
        {
            return (await WaitForNextEvent(), default);
        }

        if (timeToRetry <= TimeSpan.Zero)
        {
            return (NextAction.ProcessFailedEvents, partitionId);
        }

        var retryFailureAfter = Task.Delay(timeToRetry, cancellationToken);
        var readyToRead = messages.WaitToReadAsync(cancellationToken).AsTask();
        await Task.WhenAny(retryFailureAfter, readyToRead);

        if (retryFailureAfter.IsCompletedSuccessfully)
        {
            return (NextAction.ProcessFailedEvents, partitionId);
        }

        return (await WaitForNextEvent(), default);

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

                    _catchingUp = false;

                    return NextAction.ProcessNextEvent;
                }

                return NextAction.ProcessNextEvent;
            }

            throw new OperationCanceledException("Channel was closed");
        }
    }

    async Task<State> HandleNewEvent(StreamEvent evt, State state, CancellationToken deadlineToken)
    {
        if (state.ProcessorState.FailingPartitions.TryGetValue(evt.Partition, out _))
        {
            return state with
            {
                ProcessorState = state.ProcessorState.WithResult(SkippedProcessing.Instance, evt, DateTimeOffset.UtcNow)
            };
        }

        var (processorState, _) = await ProcessEventAndHandleResult(evt, state.ProcessorState, deadlineToken);
        state = state with
        {
            ProcessorState = processorState
        };
        
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

    async Task<State> CatchUpForPartition(
        State state,
        PartitionId partition,
        CancellationToken cancellationToken,
        CancellationToken deadlineToken)
    {
        var failingPartitionState = state.ProcessorState.FailingPartitions[partition];
        if (!ShouldRetryProcessing(failingPartitionState)) return state; // Should not really happen, since we explicitly wait for each partition

        var startPosition = new StreamPosition(failingPartitionState.Position.EventLogPosition.Value);
        var highWatermark = new StreamPosition(state.ProcessorState.Position.EventLogPosition.Value);


        var (events, hasMoreEvents) = await _fetcher.FetchInPartition(partition, startPosition, highWatermark, _handledTypes, cancellationToken);
        foreach (var streamEvent in events)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return state;
            }
            var (newState, processingResult) = await RetryProcessingEventAndHandleResult(
                streamEvent,
                state.ProcessorState,
                failingPartitionState.Reason,
                failingPartitionState.ProcessingAttempts,
                deadlineToken).ConfigureAwait(false);
            await PersistNewState(newState, deadlineToken);

            state = state with
            {
                ProcessorState = newState
            };

            if (processingResult.Succeeded)
            {
                continue;
            }

            // Failed, retry later
            return state;
        }

        if (hasMoreEvents) // No more events before the high water mark for this partition, remove it
        {
            state = state with { ProcessorState = state.ProcessorState.WithoutFailingPartition(partition) };
            await PersistNewState(state.ProcessorState, deadlineToken);
        }

        return state;
    }

    static bool ShouldRetryProcessing(FailingPartitionState state) => DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
}
