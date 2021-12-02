// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Exception that gets thrown when <see cref="CancellationToken.IsCancellationRequested" /> while calculating state transition events for an <see cref="IEmbedding" />. 
/// </summary>
public class CalculateStateTransitionEventsCancelled : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="CalculateStateTransitionEventsCancelled" /> class.
    /// </summary>
    /// <param name="embedding">The embedding identifier.</param>
    public CalculateStateTransitionEventsCancelled(EmbeddingId embedding)
        : base($"A cancellation was request while calculating state transition events fro embedding {embedding.Value}")
    {
    }
}