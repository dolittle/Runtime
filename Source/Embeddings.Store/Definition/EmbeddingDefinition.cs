// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Embeddings.Store.Definition
{
    /// <summary>
    /// Represents the definition of a projection.
    /// </summary>
    /// <param name="Embedding">The projection id.</param>
    /// <param name="Events">The list of projection event selectors.</param>
    /// <param name="InitialState">The initital projection state.</param>
    public record EmbeddingDefinition(EmbeddingId Embedding, IEnumerable<ProjectionEventSelector> Events, EmbeddingState InititalState);
}
