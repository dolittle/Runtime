// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Exception that gets thrown when a failure occurred while detecting embedding loop.
/// </summary>
public class DetectingEmbeddingLoopFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DetectingEmbeddingLoopFailed"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId"/>.</param>
    public DetectingEmbeddingLoopFailed()
        : base($"An error occurred while detecting embedding loop")
    {
    }
}