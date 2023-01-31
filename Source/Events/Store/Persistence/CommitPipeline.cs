// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Rudimentary.Pipelines;

namespace Dolittle.Runtime.Events.Store.Persistence;

public class CommitPipeline : ICanGetNextReadyBatch<Commit>
{
    /// <summary>
    /// Creates a new <see cref="CommitPipeline"/> using the given <see cref="EventLogSequenceNumber"/> as the initial sequence number.
    /// </summary>
    /// <param name="sequenceNumber">The initial <see cref="EventLogSequenceNumber"/> of the first event to be added to the <see cref="CommitPipeline"/>.</param>
    /// <returns>The created <see cref="CommitPipeline"/>.</returns>
    public static CommitPipeline NewFromEventLogSequenceNumber(EventLogSequenceNumber sequenceNumber) => new(new PipelineReadyBatchAggregator<Commit, CommitBuilder>(
        BatchSize,
        new CommitBuilder(sequenceNumber),
        commit => new CommitBuilder(commit.LastSequenceNumber + 1)));
    
    const int BatchSize = 1000;
    readonly IPipelineReadyBatchAggregator<Commit, CommitBuilder> _aggregator;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CommitPipeline"/> class.
    /// </summary>
    /// <param name="aggregator">The <see cref="IPipelineReadyBatchAggregator{TBatch,TBatchBuilder}"/>.</param>
    public CommitPipeline(IPipelineReadyBatchAggregator<Commit, CommitBuilder> aggregator)
    {
        _aggregator = aggregator;
    }

    /// <summary>
    /// Tries to add events from <see cref="CommitEventsRequest"/> into a batch.
    /// </summary>
    /// <param name="request">The <see cref="CommitEventsRequest"/>.</param>
    /// <param name="batchedEvents">The <see cref="BatchedItem{TItem}"/> batched events.</param>
    /// <param name="error">The error that may occur.</param>
    /// <returns>True if successfully added to batch, false if not.</returns>
    public bool TryAddEventsFrom(CommitEventsRequest request, [NotNullWhen(true)]out BatchedItem<CommittedEvents> batchedEvents, [NotNullWhen(false)]out Exception error)
    {
        var succeeded = _aggregator.TryAddToBatch(
            (builder, tcs) => !builder.TryAddEventsFrom(request, out var eventsToBeCommitted, out var error)
                ? (false, (null, error))
                : (true, (new BatchedItem<CommittedEvents>(eventsToBeCommitted, tcs.Task), error)),
            out var eventsOrError);
        (batchedEvents, error) = eventsOrError;
        return succeeded;
    }

    /// <summary>
    /// Tries to add events from <see cref="CommitAggregateEventsRequest"/> into a batch.
    /// </summary>
    /// <param name="request">The <see cref="CommitAggregateEventsRequest"/>.</param>
    /// <param name="batchedEvents">The <see cref="BatchedItem{TItem}"/> batched events.</param>
    /// <param name="error">The error that may occur.</param>
    /// <returns>True if successfully added to batch, false if not.</returns>
    public bool TryAddEventsFrom(CommitAggregateEventsRequest request, [NotNullWhen(true)]out BatchedItem<CommittedAggregateEvents> batchedEvents, [NotNullWhen(false)]out Exception error)
    {
        var succeeded = _aggregator.TryAddToBatch(
            (builder, tcs) => !builder.TryAddEventsFrom(request, out var eventsToBeCommitted, out var error)
                ? (false, (null, error))
                : (true, (new BatchedItem<CommittedAggregateEvents>(eventsToBeCommitted, tcs.Task), error)),
            out var eventsOrError);
        (batchedEvents, error) = eventsOrError;
        return succeeded;
    }

    /// <inheritdoc />
    public bool TryGetNextBatch([NotNullWhen(true)] out ReadyBatch<Commit> batch)
        => _aggregator.TryGetNextBatch(out batch);

    /// <inheritdoc />
    public void EmptyAllWithFailure(Exception failure)
        => _aggregator.EmptyAllWithFailure(failure);
}
