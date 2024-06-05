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
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Proto;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using StreamProcessorState = Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

public class ConcurrentPartitionedProcessor : ProcessorBase<StreamProcessorState>
{
    readonly ICanFetchEventsFromPartitionedStream _fetcher;

    internal delegate State ReceiveResult(State current);

    internal class ActiveRequests
    {
        readonly int _concurrency;
        readonly HashSet<PartitionId> _currentPartitions = new();
        readonly Channel<(PartitionId partition, Task<ReceiveResult> callback)> _currentlyProcessing;
        readonly ChannelReader<(PartitionId partition, Task<ReceiveResult> callback)> _reader;

        public IReadOnlySet<PartitionId> PartitionsBeingProcessed => _currentPartitions;

        public ActiveRequests(int concurrency)
        {
            _concurrency = concurrency;
            _currentlyProcessing = Channel.CreateUnbounded<(PartitionId, Task<ReceiveResult>)>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = true
            });
            _reader = _currentlyProcessing.Reader;
        }

        public bool IsProcessing(PartitionId partitionId) => _currentPartitions.Contains(partitionId);

        public ValueTask Add(PartitionId partitionId, Task<ReceiveResult> callBack)
        {
            if (!_currentPartitions.Add(partitionId)) throw new ArgumentException($"Partition {partitionId} is already being processed");
            return _currentlyProcessing.Writer.WriteAsync((partitionId, callBack));
        }

        public ValueTask AddSkipped(Task<ReceiveResult> callBack)
        {
            return _currentlyProcessing.Writer.WriteAsync((PartitionId.None, callBack));
        }

        public bool HasCompletedRequests()
        {
            return _reader.TryPeek(out var next) && next.callback.IsCompleted;
        }

        public async Task WaitForNextCompleted(CancellationToken cancellationToken)
        {
            await _reader.WaitToReadAsync(cancellationToken);
            _reader.TryPeek(out var next);
            await next.callback;
        }

        public async ValueTask<Task<ReceiveResult>> GetNextCompleted(CancellationToken cancellationToken)
        {
            var (partition, callback) = await _reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            if (partition != PartitionId.None)
            {
                _currentPartitions.Remove(partition);
            }

            return callback;
        }

        public bool IsEmpty => _currentPartitions.Count == 0;
        public bool IsFull => _currentPartitions.Count >= _concurrency;
        public int Count => _currentPartitions.Count;
    }

    internal record State(StreamProcessorState ProcessorState, ActiveRequests ActiveRequests)
    {
        public bool NoFailingEvents => ProcessorState.FailingPartitions.IsEmpty;

        public bool TryGetTimeToRetry(ActiveRequests activeRequests, out TimeSpan timeToRetry, [NotNullWhen(true)] out PartitionId? selectedPartitionId)
        {
            timeToRetry = TimeSpan.MaxValue;
            selectedPartitionId = default;
            if (NoFailingEvents) return false;
            var processing = activeRequests.PartitionsBeingProcessed;
            foreach (var (partitionId, failingPartitionState) in ProcessorState.FailingPartitions)
            {
                if (processing.Contains(partitionId)) continue;
                if (failingPartitionState.TryGetTimespanToRetry(out var partitionTimeToRetry) && partitionTimeToRetry < timeToRetry)
                {
                    timeToRetry = partitionTimeToRetry;
                    selectedPartitionId = partitionId;
                }
            }

            return timeToRetry < TimeSpan.MaxValue;
        }

        /// <summary>
        /// Mark the event as skipped. Used when the event partition is failing, and the event should be retried out of band
        /// </summary>
        /// <param name="streamEvent"></param>
        /// <returns></returns>
        public State WithSkippedEvent(StreamEvent streamEvent) =>
            this with { ProcessorState = ProcessorState.WithResult(SkippedProcessing.Instance, streamEvent, DateTimeOffset.UtcNow) };
        
        /// <summary>
        /// When there are unhandled events in the event log, the processor state should be updated to reflect that the event was skipped.
        /// </summary>
        /// <param name="nextProcessingPosition"></param>
        /// <returns></returns>
        public State WithNextProcessingPosition(EventLogSequenceNumber nextProcessingPosition) =>
            this with { ProcessorState = ProcessorState.WithNextEventLogSequence(nextProcessingPosition) };
    }

    readonly ImmutableHashSet<Guid> _handledTypes;
    readonly int _concurrency;


    public ConcurrentPartitionedProcessor(
        StreamProcessorId streamProcessorId,
        IEnumerable<ArtifactId> handledEventTypes,
        IEventProcessor processor,
        IStreamProcessorStates streamProcessorStates,
        ExecutionContext executionContext,
        ScopedStreamProcessorProcessedEvent onProcessed,
        ScopedStreamProcessorFailedToProcessEvent onFailedToProcess,
        TenantId tenantId,
        ICanFetchEventsFromPartitionedStream fetcher,
        int concurrency,
        ILogger logger)
        :
        base(
            streamProcessorId, processor, streamProcessorStates, executionContext, onProcessed, onFailedToProcess, tenantId, logger)
    {
        _fetcher = fetcher;
        _concurrency = concurrency;
        _handledTypes = handledEventTypes.Select(_ => _.Value).ToImmutableHashSet();
    }

    public async Task Process(ChannelReader<(StreamEvent? streamEvent, EventLogSequenceNumber nextSequenceNumber)> messages, IStreamProcessorState state, CancellationToken cancellationToken,
        CancellationToken deadlineToken)
    {
        var currentState = new State(AsPartitioned(state), new ActiveRequests(_concurrency));
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var (nextAction, partitionId) = await WaitForNextAction(messages, currentState, cancellationToken);
                try
                {
                    switch (nextAction)
                    {
                        case NextAction.ReceiveResult:
                            currentState = await ProcessReceiveResult(currentState, cancellationToken, deadlineToken);
                            break;

                        case NextAction.ProcessNextEvent:
                            currentState = await ProcessNextEvent(messages, currentState, cancellationToken, deadlineToken);

                            break;
                        case NextAction.ProcessFailedEvent:
                            currentState = await CatchUpForPartition(currentState, partitionId!, cancellationToken, deadlineToken);

                            break;
                        case NextAction.Completed:
                            return;
                    }
                }
                finally
                {
                    await PersistNewState(currentState.ProcessorState, deadlineToken);
                }
            }
        }
        finally
        {
            // If there are requests in-flight, let's try to wait for them to complete

            await WaitForCompletions(currentState, deadlineToken);
        }
    }

    async Task WaitForCompletions(State currentState, CancellationToken deadlineToken)
    {
        if (currentState.ActiveRequests.IsEmpty)
            return;

        Logger.WaitingForCompletions(Identifier.EventProcessorId, Identifier.ScopeId, currentState.ActiveRequests.Count);

        try
        {
            var timeout = CancellationTokens.FromSeconds(10);
            while (!currentState.ActiveRequests.IsEmpty && !deadlineToken.IsCancellationRequested)
            {
                currentState = await ProcessReceiveResult(currentState, timeout, deadlineToken);
            }

            Logger.FinishedWaitingForCompletions(Identifier.EventProcessorId, Identifier.ScopeId);
        }
        catch (Exception e)
        {
            Logger.FailedWaitingForCompletions(e, Identifier.EventProcessorId, Identifier.ScopeId, currentState.ActiveRequests.Count);
        }
    }

    async Task<State> ProcessNextEvent(ChannelReader<(StreamEvent? streamEvent, EventLogSequenceNumber nextSequenceNumber)> messages, State currentState, CancellationToken stoppingToken,
        CancellationToken deadlineToken)
    {
        var (evt, nextProcessingPosition) = await messages.ReadAsync(stoppingToken);
        
        if (evt is not null)
        {
            // Event is present, try to process it

            if (currentState.ProcessorState.FailingPartitions.TryGetValue(evt.Partition, out _))
            {
                await currentState.ActiveRequests.AddSkipped(Task.FromResult(AsSkippedEvent(evt)));
                return currentState;
            }

            var newTask = ProcessEventAndReturnStateUpdateCallback(evt, deadlineToken);
            await currentState.ActiveRequests.Add(evt.Partition, newTask).ConfigureAwait(false);
        }
        else
        {
            // Eventlog has unhandled events, skip them
            currentState = currentState.WithNextProcessingPosition(nextProcessingPosition);
            await PersistNewState(currentState.ProcessorState, deadlineToken);
        }
        
        return currentState;
    }

    ReceiveResult AsSkippedEvent(StreamEvent evt) => current => current.WithSkippedEvent(evt);

    async Task<State> ProcessReceiveResult(State currentState, CancellationToken cancellationToken, CancellationToken deadlineToken)
    {
        var readAsync = await currentState.ActiveRequests.GetNextCompleted(cancellationToken);
        var receive = await readAsync;
        currentState = receive(currentState);
        await PersistNewState(currentState.ProcessorState, deadlineToken);
        return currentState;
    }

    async Task<ReceiveResult> ProcessEventAndReturnStateUpdateCallback(StreamEvent evt, CancellationToken cancellationToken)
    {
        var (processingResult, elapsed) = await ProcessEvent(evt, cancellationToken);

        return state =>
        {
            var updatedState = HandleProcessingResult(processingResult, evt, elapsed, state.ProcessorState);

            return state with { ProcessorState = updatedState };
        };
    }

    internal enum NextAction
    {
        /// <summary>
        /// Process the next event in the event stream.
        /// </summary>
        ProcessNextEvent,
        
        /// <summary>
        /// Receive the result of the current event being processed
        /// </summary>
        ReceiveResult,

        /// <summary>
        /// Retry processing of failed events.
        /// </summary>
        ProcessFailedEvent,

        /// <summary>
        /// Processing is complete.
        /// </summary>
        Completed,
    }

    /// <summary>
    /// Determines which action to take next.
    /// If the current processing is complete, it will return <see cref="NextAction.ReceiveResult"/>.
    /// Else it will wait for new data to become available or for the retry delay to expire.
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    internal static async ValueTask<(NextAction, PartitionId?)> WaitForNextAction(
        ChannelReader<(StreamEvent? streamEvent, EventLogSequenceNumber nextSequenceNumber)> messages,
        State state,
        CancellationToken cancellationToken)
    {
        Task? resultReady = null;

        if (state.ActiveRequests.IsFull || state.ActiveRequests.HasCompletedRequests())
        {
            return (NextAction.ReceiveResult, default);
        }

        if (!state.ActiveRequests.IsEmpty)
        {
            resultReady = state.ActiveRequests.WaitForNextCompleted(cancellationToken);
        }

        Task? retryAfter = null;

        if (state.TryGetTimeToRetry(state.ActiveRequests, out var timeToRetry, out var partitionId))
        {
            if (timeToRetry <= TimeSpan.Zero)
            {
                return (NextAction.ProcessFailedEvent, partitionId);
            }

            // Not ready to retry yet
            retryAfter = Task.Delay(timeToRetry, cancellationToken);
        }

        var tasks = new List<Task>(3);
        if (resultReady != null)
        {
            tasks.Add(resultReady);
        }

        if (retryAfter != null)
        {
            tasks.Add(retryAfter);
        }

        var readyToRead = messages.WaitToReadAsync(cancellationToken).AsTask();
        tasks.Add(readyToRead);

        await Task.WhenAny(tasks);

        // Prioritize processing of results over retrying
        if (resultReady is not null && resultReady.IsCompleted)
        {
            return (NextAction.ReceiveResult, partitionId);
        }

        // If a partition is ready to retry, do that
        if (retryAfter is not null && retryAfter.IsCompletedSuccessfully)
        {
            return (NextAction.ProcessFailedEvent, partitionId);
        }

        // Else check if there are more events to process
        var hasMoreEvents = readyToRead.IsCompleted && readyToRead.Result;

        // Channel has closed, processor is terminating
        if (!hasMoreEvents)
        {
            if (state.ActiveRequests.IsEmpty)
            {
                return (NextAction.Completed, default);
            }

            return (NextAction.ReceiveResult, default);
        }

        if (messages.TryPeek(out var message) && message.streamEvent is not null && state.ActiveRequests.IsProcessing(message.streamEvent.Partition))
        {
            return (NextAction.ReceiveResult, message.streamEvent.Partition);
        }

        return (NextAction.ProcessNextEvent, default);
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

    async Task<State> CatchUpForPartition(State state,
        PartitionId partition,
        CancellationToken cancellationToken, CancellationToken deadlineToken)
    {
        var failingPartitionState = state.ProcessorState.FailingPartitions[partition];
        if (!ShouldRetryProcessing(failingPartitionState)) return state; // Should not really happen, since we explicitly wait for each partition

        var startPosition = new StreamPosition(failingPartitionState.Position.EventLogPosition.Value);
        var highWatermark = new StreamPosition(state.ProcessorState.Position.EventLogPosition.Value);

        var (evt, _) = await _fetcher.FetchNextEventInPartition(partition, startPosition, highWatermark, _handledTypes, cancellationToken);

        if (evt is null)
        {
            // No more events before the high water mark for this partition, remove it
            state = state with { ProcessorState = state.ProcessorState.WithoutFailingPartition(partition) };
            await PersistNewState(state.ProcessorState, deadlineToken);
            return state;
        }

        var newTask = ProcessEventAndReturnStateUpdateCallback(evt, deadlineToken);
        await state.ActiveRequests.Add(evt.Partition, newTask);
        return state;
    }

    static bool ShouldRetryProcessing(FailingPartitionState state) => DateTimeOffset.UtcNow.CompareTo(state.RetryTime) >= 0;
}
