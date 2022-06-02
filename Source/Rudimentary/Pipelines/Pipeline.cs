// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Represents an implementation of <see cref="IPipeline{TBatchItem,TBatch}"/>.
/// </summary>
/// <typeparam name="TBatchItem">The <see cref="Type"/> of items to include in the batch.</typeparam>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch.</typeparam>
public class Pipeline<TBatchItem, TBatch> : IPipeline<TBatchItem, TBatch>
{
    readonly Func<TBatch, ICanAddToABatch<TBatchItem, TBatch>> _createBatchBuilderFromPreviousBatch;
    readonly Channel<ReadyBatch<TBatch>> _preparedBatches = Channel.CreateUnbounded<ReadyBatch<TBatch>>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = true
    });

    ICanAddToABatch<TBatchItem, TBatch> _batchBuilder;
    TaskCompletionSource<Try> _completionSource;
    
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
    {
        BatchSize = batchSize;
        _createBatchBuilderFromPreviousBatch = createBatchBuilderFromPreviousBatch;
        _batchBuilder = initialBatchBuilder;
        _completionSource = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
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

    /// <inheritdoc />
    public int BatchSize { get; }

    /// <inheritdoc />
    public bool TryAdd(TBatchItem item, out BatchedItem<TBatchItem> batchedItem, out Exception error)
    {
        batchedItem = default;
        if (!_batchBuilder.TryAddToBatch(item, out error))
        {
            return false;
        }
        var tcs = _completionSource;
        ReplaceBatchIfFull();
        batchedItem = new BatchedItem<TBatchItem>(item, tcs.Task);
        return true;
    }

    /// <inheritdoc />
    public bool TryGetNextBatch(out ReadyBatch<TBatch> readyBatch)
    {
        if (_preparedBatches.Reader.TryRead(out readyBatch))
        {
            return true;
        }

        if (_batchBuilder.BatchIsEmpty)
        {
            return false;
        }
        readyBatch = BuildAndReplaceCurrentBatch();
        return true;
    }
    
    void ReplaceBatchIfFull()
    {
        if (_batchBuilder.Count < BatchSize)
        {
            return;
        }
        var batch = BuildAndReplaceCurrentBatch();
        _preparedBatches.Writer.TryWrite(batch);
    }

    ReadyBatch<TBatch> BuildAndReplaceCurrentBatch()
    {
        var batch = _batchBuilder.Build();
        var tcs = _completionSource;
        _batchBuilder = _createBatchBuilderFromPreviousBatch(batch);
        _completionSource = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
        return new ReadyBatch<TBatch>(batch, tcs);
    }
}
