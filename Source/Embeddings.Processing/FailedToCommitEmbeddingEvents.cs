// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Exception that gets thrown when embedding events failed to be committed.
/// </summary>
public class FailedToCommitEmbeddingEvents : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedToCommitEmbeddingEvents"/> class.
    /// </summary>
    /// <param name="failure">The <see cref="Failure"/>.</param>
    public FailedToCommitEmbeddingEvents(Failure failure)
        : base($"Failed to commit embedding events because {failure}")
    {}
}
