// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Defines a system that aggregates batches.
/// </summary>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch.</typeparam>
/// <typeparam name="TBatchBuilder">The <see cref="Type"/> of the batch builder.</typeparam>
public interface IPipelineReadyBatchAggregator<TBatch, out TBatchBuilder> : ICanGetNextReadyBatch<TBatch>
    where TBatchBuilder : ICanBuildABatch<TBatch>
{
    /// <summary>
    /// Gets the batch size of the pipeline.
    /// </summary>
    public int BatchSize { get; }

    /// <summary>
    /// Given a <see cref="Func{TResult}"/> it correctly batch the <typeparamref name="TBatchBuilder"/>.
    /// </summary>
    /// <param name="addToBatch">The func for adding an items to a batch.</param>
    public bool TryAddToBatch<TResult>(Func<TBatchBuilder, TaskCompletionSource<Try>, (bool, TResult)> addToBatch, out TResult result);
}
