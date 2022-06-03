// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Defines a pipeline for creating batches of items.
/// </summary>
/// <typeparam name="TBatchItem">The <see cref="Type"/> of the items to put in a batch.</typeparam>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch</typeparam>
public interface IPipeline<TBatchItem, TBatch> : ICanGetNextReadyBatch<TBatch>
{
    /// <summary>
    /// Gets the batch size of the pipeline.
    /// </summary>
    public int BatchSize { get; }

    /// <summary>
    /// Tries to add the input to the pipeline.
    /// </summary>
    /// <param name="item">What to put in a batch.</param>
    /// <param name="batchedItem">The batched item.</param>
    /// <param name="error">The error.</param>
    /// <returns>True if successfully added to pipeline, false if not.</returns>
    bool TryAdd(TBatchItem item, out BatchedItem<TBatchItem> batchedItem, out Exception error);
}
