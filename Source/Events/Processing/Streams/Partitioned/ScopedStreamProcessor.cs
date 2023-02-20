// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned;

/// <summary>
/// Represents an implementation of <see cref="AbstractScopedStreamProcessor" /> that processes an partitioned stream of events.
/// </summary>
public class ScopedStreamProcessor : AbstractScopedStreamProcessor
{
    readonly IStreamProcessorStates _streamProcessorStates;
    readonly IFailingPartitions _failingPartitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedStreamProcessor"/> class.
    /// </summary>
    /// <param name="tenantId">The <see cref="TenantId"/>.</param>
    /// <param name="streamProcessorId">The <see cref="IStreamProcessorId" />.</param>
    /// <param name="sourceStreamDefinition">The source stream <see cref="StreamDefinition" />.</param>
    /// <param name="initialState">The <see cref="StreamProcessorState" />.</param>
    /// <param name="processor">An <see cref="IEventProcessor" /> to process the event.</param>
    /// <param name="streamProcessorStates">The <see cref="IStreamProcessorStates" />.</param>
    /// <param name="eventsFromStreamsFetcher">The<see cref="ICanFetchEventsFromStream" />.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> of the stream processor.</param>
    /// <param name="failingPartitionsFactory">The factory to use to create the <see cref="IFailingPartitions" />.</param>
    /// <param name="eventFetcherPolicies">The policies to use while fetching events.</param>
    /// <param name="streamWatcher">The <see cref="IStreamEventWatcher" />.</param>
    /// <param name="timeToRetryGetter">The <see cref="ICanGetTimeToRetryFor{T}" /> <see cref="StreamProcessorState" />.</param>
    /// <param name="logger">An <see cref="ILogger" /> to log messages.</param>
    public ScopedStreamProcessor(
        TenantId tenantId,
        IStreamProcessorId streamProcessorId,
        IStreamDefinition sourceStreamDefinition,
        StreamProcessorState initialState,
        IEventProcessor processor,
        IStreamProcessorStates streamProcessorStates,
        ICanFetchEventsFromPartitionedStream eventsFromStreamsFetcher,
        ExecutionContext executionContext,
        Func<IEventProcessor, ICanFetchEventsFromPartitionedStream, Func<StreamEvent, ExecutionContext>, IFailingPartitions> failingPartitionsFactory,
        IEventFetcherPolicies eventFetcherPolicies,
        IStreamEventWatcher streamWatcher,
        ILogger logger)
        : base(tenantId, streamProcessorId, sourceStreamDefinition, initialState, processor, eventsFromStreamsFetcher, executionContext, eventFetcherPolicies, streamWatcher, logger)
    {
        _streamProcessorStates = streamProcessorStates;
        _failingPartitions = failingPartitionsFactory(processor, eventsFromStreamsFetcher, GetExecutionContextForEvent);
    }

    /// <inheritdoc/>
    protected override async Task<IStreamProcessorState> ProcessEvents(IEnumerable<(StreamEvent, ExecutionContext)> events, IStreamProcessorState currentState,
        CancellationToken cancellationToken)
    {
        var streamProcessorState = currentState as StreamProcessorState;

        foreach (var (@event, executionContext) in events)
        {
            if (streamProcessorState.FailingPartitions.ContainsKey(@event.Partition))
            {
                var newState = streamProcessorState with
                {
                    Position = @event.NextProcessingPosition,
                };
                await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
                streamProcessorState = newState;
            }
            else
            {
                var (newState, _) = await ProcessEvent(@event, streamProcessorState, executionContext, cancellationToken).ConfigureAwait(false);
                streamProcessorState = newState as StreamProcessorState;
            }
        }

        return streamProcessorState;
    }

    /// <inheritdoc/>
    protected override async Task<IStreamProcessorState> Catchup(IStreamProcessorState currentState, CancellationToken cancellationToken) =>
        await _failingPartitions.CatchupFor(Identifier, currentState as StreamProcessorState, cancellationToken);

    /// <inheritdoc/>
    protected override async Task<IStreamProcessorState> OnFailedProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent,
        IStreamProcessorState currentState)
    {
        var newState = currentState.WithFailure(failedProcessing, processedEvent, DateTimeOffset.MaxValue);
        await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None);
        return newState;
    }

    /// <inheritdoc/>
    protected override async Task<IStreamProcessorState> OnRetryProcessingResult(FailedProcessing failedProcessing, StreamEvent processedEvent,
        IStreamProcessorState currentState)
    {
        var newState = currentState.WithFailure(failedProcessing, processedEvent, DateTimeOffset.UtcNow.Add(failedProcessing.RetryTimeout));
        await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None);
        return newState;
    }

    /// <inheritdoc/>
    protected override async Task<IStreamProcessorState> OnSuccessfulProcessingResult(SuccessfulProcessing successfulProcessing, StreamEvent processedEvent,
        IStreamProcessorState currentState)
    {
        var newState = currentState.WithSuccessfullyProcessed(processedEvent, DateTimeOffset.UtcNow);
        await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
        return newState;
    }

    /// <inheritdoc />
    protected override async Task<IStreamProcessorState> SetNewStateWithPosition(IStreamProcessorState currentState, ProcessingPosition position)
    {
        var state = currentState as StreamProcessorState;
        var newState = new StreamProcessorState(
            position,
            FailingPartitionsIgnoringPartitionsToReprocess(state, position),
            state.LastSuccessfullyProcessed);
        await _streamProcessorStates.Persist(Identifier, newState, CancellationToken.None).ConfigureAwait(false);
        return newState;
    }

    static ImmutableDictionary<PartitionId, FailingPartitionState> FailingPartitionsIgnoringPartitionsToReprocess(StreamProcessorState state, ProcessingPosition position)
        => state.FailingPartitions
            .Where(_ => _.Value.Position.StreamPosition < position.StreamPosition)
            .ToImmutableDictionary(_ => _.Key, _ => _.Value);
}
