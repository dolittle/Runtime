// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Represents an implementation of <see cref="IPipelineReadyBatchAggregator{TBatch,TBatchBuilder}"/>.
/// </summary>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch.</typeparam>
/// <typeparam name="TBatchBuilder">The <see cref="Type"/> of the batch builder.</typeparam>
public class PipelineReadyBatchAggregator<TBatch, TBatchBuilder> : IPipelineReadyBatchAggregator<TBatch, TBatchBuilder>
    where TBatchBuilder : ICanBuildABatch<TBatch>
{
    readonly Func<TBatch, TBatchBuilder> _createBatchBuilderFromPreviousBatch;
    readonly Channel<ReadyBatch<TBatch>> _preparedBatches = Channel.CreateUnbounded<ReadyBatch<TBatch>>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = true
    });
    TBatchBuilder _currentBatchBuilder;
    TaskCompletionSource<Try> _completionSource;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineReadyBatchAggregator{TBatch,TBatchBuilder}"/> class.
    /// </summary>
    /// <param name="batchSize">The max size of a batch.</param>
    /// <param name="currentBatchBuilder">The initial batch builder.</param>
    /// <param name="createBatchBuilderFromPreviousBatch">The <see cref="Func{T, TResult}"/> for creating a <typeparamref name="TBatchBuilder"/> from the previously built <typeparamref name="TBatch"/>.</param>
    public PipelineReadyBatchAggregator(int batchSize, TBatchBuilder currentBatchBuilder, Func<TBatch, TBatchBuilder> createBatchBuilderFromPreviousBatch)
    {
        BatchSize = batchSize;
        _currentBatchBuilder = currentBatchBuilder;
        _createBatchBuilderFromPreviousBatch = createBatchBuilderFromPreviousBatch;
        _completionSource = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <inheritdoc />
    public int BatchSize { get; }
    
    /// <inheritdoc />
    public bool TryAddToBatch<TResult>(Func<TBatchBuilder, TaskCompletionSource<Try>, (bool, TResult)> addToBatch, out TResult result)
    {
        var tcs = _completionSource;
        (var succeeded, result) = addToBatch(_currentBatchBuilder, tcs); 
        if (succeeded)
        {
            ReplaceBatchIfFull();
        }
        return succeeded;
    }

    /// <inheritdoc />
    public bool TryGetNextBatch(out ReadyBatch<TBatch> readyBatch)
    {
        if (_preparedBatches.Reader.TryRead(out readyBatch))
        {
            return true;
        }

        if (_currentBatchBuilder.BatchIsEmpty)
        {
            return false;
        }
        readyBatch = BuildAndReplaceCurrentBatch();
        return true;
    }

    /// <inheritdoc />
    public void EmptyAllWithFailure(Exception failure)
    {
        while (TryGetNextBatch(out var batch))
        {
            batch.Fail(failure);
        }
    }

    void ReplaceBatchIfFull()
    {
        if (_currentBatchBuilder.Count < BatchSize)
        {
            return;
        }
        var batch = BuildAndReplaceCurrentBatch();
        _preparedBatches.Writer.TryWrite(batch);
    }

    ReadyBatch<TBatch> BuildAndReplaceCurrentBatch()
    {
        var batch = _currentBatchBuilder.Build();
        var tcs = _completionSource;
        _currentBatchBuilder = _createBatchBuilderFromPreviousBatch(batch);
        _completionSource = new TaskCompletionSource<Try>(TaskCreationOptions.RunContinuationsAsynchronously);
        return new ReadyBatch<TBatch>(batch, tcs);
    }
}
