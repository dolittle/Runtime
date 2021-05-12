// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Embeddings.Store.Definition
{
    /// <summary>
    /// Represents the definition of an embedding.
    /// </summary>
    /// <param name="Embedding">The embedding id.</param>
    /// <param name="Events">The list of the embeddings event selectors.</param>
    /// <param name="InitialState">The initital embedding state.</param>
    public record EmbeddingDefinition(EmbeddingId Embedding, IEnumerable<ProjectionEventSelector> Events, ProjectionState InititalState);
}
