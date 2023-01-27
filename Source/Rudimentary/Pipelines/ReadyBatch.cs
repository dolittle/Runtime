// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Represents a batch of items.
/// </summary>
/// <param name="Batch">The batch item </param>
/// <param name="Completion">The <see cref="TaskCompletionSource"/> representing the success of the batch flight.</param>
/// <typeparam name="TBatch">The <see cref="Type"/> of the ready batch.</typeparam>
public record ReadyBatch<TBatch>(TBatch Batch, TaskCompletionSource<Try> Completion)
{
    /// <summary>
    /// Completes the batch.
    /// </summary>
    public void Complete() => Completion.TrySetResult(Try.Succeeded);

    /// <summary>
    /// Fails the batch.
    /// </summary>
    /// <param name="reason">The reason it failed.</param>
    public void Fail(Exception reason) => Completion.TrySetResult(Try.Failed(reason));
}
