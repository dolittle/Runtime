// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Dolittle.Runtime.Rudimentary.Pipelines;

/// <summary>
/// Defines a system that can get the next ready batch.
/// </summary>
/// <typeparam name="TBatch">The <see cref="Type"/> of the batch.</typeparam>
public interface ICanGetNextReadyBatch<TBatch>
{
    /// <summary>
    /// Tries to get the next <see cref="ReadyBatch{TBatch}"/> to send.
    /// </summary>
    /// <param name="readyBatch">The next <see cref="ReadyBatch{TBatch}"/>.</param>
    /// <returns>True if there was a ready batch, false if not.</returns>
    bool TryGetNextBatch([NotNullWhen(true)] out ReadyBatch<TBatch>? readyBatch);
    
    /// <summary>
    /// Fails all ready batches.
    /// </summary>
    /// <param name="failure">The failure.</param>
    void EmptyAllWithFailure(Exception failure);
}
