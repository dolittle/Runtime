// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Defines a system that can build a batch from a single input type.
/// </summary>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch.</typeparam>
public interface ICanBuildABatch<out TBatch>
{
    /// <summary>
    /// The number of items in the batch.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets a value indicating whether the batch is empty.
    /// </summary>
    public bool BatchIsEmpty { get; }

    /// <summary>
    /// Builds the batch.
    /// </summary>
    /// <returns>The <typeparamref name="TBatch"/>.</returns>
    TBatch Build();
}
