// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

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
