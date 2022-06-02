// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Defines a system that can build a batch from a single input type.
/// </summary>
/// <typeparam name="TBatchItem">The <see cref="Type"/> of items to include in the batch.</typeparam>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch.</typeparam>
public interface ICanAddToABatch<in TBatchItem, out TBatch> : ICanBuildABatch<TBatch>
{
    /// <summary>
    /// Tries to add <typeparamref name="TBatchItem"/> to the batch.
    /// </summary>
    /// <param name="input">The input to add to the batch.</param>
    /// <param name="error">The error if it occurred.</param>
    /// <returns>True if successfully added to batch, false if not.</returns>
    bool TryAddToBatch(TBatchItem input, [NotNullWhen(false)]out Exception error);
}

public class PipelineBatchAggregator<TBatch> : ICanGetNextReadyBatch<TBatch>
{
    readonly Func<TBatch, ICanAddToABatch<TBatchItem, TBatch>> _createBatchBuilderFromPreviousBatch;
    readonly Channel<ReadyBatch<TBatch>> _preparedBatches = Channel.CreateUnbounded<ReadyBatch<TBatch>>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = true
    });
    
    
    public PipelineBatchAggregator(
        int batchSize,
        Func<TBatch, ICanBuildABatch<TBatch>> createBatchBuilderFromPreviousBatch)
    {
        BatchSize = batchSize;
        _createBatchBuilderFromPreviousBatch = createBatchBuilderFromPreviousBatch;
        _batchBuilder = initialBatchBuilder;
        _completionSource = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
    }
    
    /// <inheritdoc />
    public int BatchSize { get; }
    
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
