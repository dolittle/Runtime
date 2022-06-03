// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Represents an implementation of <see cref="IPipeline{TBatchItem,TBatch}"/>.
/// </summary>
/// <typeparam name="TBatchItem">The <see cref="Type"/> of items to include in the batch.</typeparam>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch.</typeparam>
public class Pipeline<TBatchItem, TBatch> : Pipeline<TBatchItem, TBatch, ICanAddToABatch<TBatchItem, TBatch>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Pipeline{TBatchItem,TBatch}"/> class.
    /// </summary>
    /// <param name="batchSize">The max size of a batch.</param>
    /// <param name="initialBatchBuilder">The initial batch builder.</param>
    /// <param name="createBatchBuilderFromPreviousBatch">The <see cref="Func{T, TResult}"/> for creating a <see cref="ICanAddToABatch{TBatchItem,TBatch}"/> from the previously built <typeparamref name="TBatch"/>.</param>
    public Pipeline(
        int batchSize,
        ICanAddToABatch<TBatchItem, TBatch> initialBatchBuilder,
        Func<TBatch, ICanAddToABatch<TBatchItem, TBatch>> createBatchBuilderFromPreviousBatch)
        : base(new PipelineReadyBatchAggregator<TBatch, ICanAddToABatch<TBatchItem, TBatch>>(batchSize, initialBatchBuilder, createBatchBuilderFromPreviousBatch))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Pipeline{TBatchItem,TBatch}"/> class.
    /// </summary>
    /// <param name="batchSize">The max size of a batch.</param>
    /// <param name="createBatchBuilder">The <see cref="Func{TResult}"/> for creating a <see cref="ICanAddToABatch{TBatchItem,TBatch}"/>.</param>
    public Pipeline(
        int batchSize,
        Func<ICanAddToABatch<TBatchItem, TBatch>> createBatchBuilder)
        : this(batchSize, createBatchBuilder(), _ => createBatchBuilder())
    { }
}

/// <summary>
/// Represents an implementation of <see cref="IPipeline{TBatchItem,TBatch}"/>.
/// </summary>
/// <typeparam name="TBatchItem">The <see cref="Type"/> of items to include in the batch.</typeparam>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch.</typeparam>
/// <typeparam name="TBatchBuilder">The <see cref="Type"/> of the batch builder</typeparam>
public class Pipeline<TBatchItem, TBatch, TBatchBuilder> : IPipeline<TBatchItem, TBatch>
    where TBatchBuilder : ICanAddToABatch<TBatchItem, TBatch>
{
    readonly IPipelineReadyBatchAggregator<TBatch, TBatchBuilder> _aggregator;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Pipeline{TBatchItem,TBatch}"/> class.
    /// </summary>
    /// <param name="aggregator">The pipeline ready batch aggregator.</param>
    public Pipeline(IPipelineReadyBatchAggregator<TBatch, TBatchBuilder> aggregator)
    {
        _aggregator = aggregator;
        BatchSize = _aggregator.BatchSize;
    }

    /// <inheritdoc />
    public int BatchSize { get; }

    /// <inheritdoc />
    public bool TryAdd(TBatchItem item, out BatchedItem<TBatchItem> batchedItem, out Exception error)
    {
        batchedItem = default;
        error = default;
        var succeeded = _aggregator.TryAddToBatch(
            (builder, tcs) => !builder.TryAddToBatch(item, out var error)
                ? (false, (null, error))
                : (true, (new BatchedItem<TBatchItem>(item, tcs.Task), error)),
            out var itemAndError);
        (batchedItem, error) = itemAndError;
        return succeeded;
    }

    /// <inheritdoc />
    public bool TryGetNextBatch(out ReadyBatch<TBatch> readyBatch)
        => _aggregator.TryGetNextBatch(out readyBatch);

    public void EmptyAllWithFailure(Exception failure)
        => _aggregator.EmptyAllWithFailure(failure);
}
