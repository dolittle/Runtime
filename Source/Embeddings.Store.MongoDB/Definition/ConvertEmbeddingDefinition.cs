// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store.MongoDB.Definition;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.Definition
{
    /// <summary>
    /// Represents an implementation of <see cref="IConvertEmbeddingDefinition" />.
    /// </summary>
    [Singleton]
    public class ConvertEmbeddingDefinition : IConvertEmbeddingDefinition
    {
        /// <inheritdoc/>
        public Store.Definition.EmbeddingDefinition ToRuntime(EmbeddingDefinition definition)
            => new(
                definition.Embedding,
                definition.EventSelectors.Select(_ => new Projections.Store.Definition.ProjectionEventSelector(
                    _.EventType,
                    _.EventKeySelectorType,
                    _.EventKeySelectorExpression)),
                definition.InitialState);

        /// <inheritdoc/>
        public EmbeddingDefinition ToStored(Store.Definition.EmbeddingDefinition definition)
            => new()
            {
                Embedding = definition.Embedding,
                InitialState = definition.InititalState,
                EventSelectors = definition.Events.Select(_ => new ProjectionEventSelector
                {
                    EventKeySelectorType = _.KeySelectorType,
                    EventKeySelectorExpression = _.KeySelectorExpression,
                    EventType = _.EventType,
                }).ToArray()
            };
    }
}
