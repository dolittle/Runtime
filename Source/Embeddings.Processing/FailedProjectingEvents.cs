
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Exception that gets thrown when no events could be projected.
/// </summary>
public class FailedProjectingEvents : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedProjectingEvents"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
    /// <param name="inner">The inner<see cref="Exception"/> </param>
    public FailedProjectingEvents(EmbeddingId embedding, Exception inner)
        : base($"A failure occurred while projecting events for embedding {embedding.Value}", inner)
    {
    }
}