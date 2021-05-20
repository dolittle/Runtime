// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents the runtime representation of the embedding registartion arguments.
    /// </summary>
    /// <param name="ExecutionContext">The execution context.</param>
    /// <param name="EmbeddingId">The embedding identifier.</param>
    /// <param name="Events">The event types.</param>
    /// <param name="Events">The initial projection state.</param>
    public record EmbeddingRegistrationArguments(
        ExecutionContext ExecutionContext,
        EmbeddingId EmbeddingId,
        IEnumerable<Artifact> Events,
        ProjectionState InitialState);
}
