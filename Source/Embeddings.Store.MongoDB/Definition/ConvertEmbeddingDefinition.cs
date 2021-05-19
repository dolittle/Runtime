// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store.MongoDB.Definition;
using Dolittle.Runtime.Projections.Store.State;
using MongoDB.Bson;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.Definition
{
    /// <summary>
    /// Represents an implementation of <see cref="IConvertEmbeddingDefinition" />.
    /// </summary>
    [Singleton]
    public class ConvertEmbeddingDefinition : IConvertEmbeddingDefinition
    {
        public Store.Definition.EmbeddingDefinition ToRuntime(
            EmbeddingId embedding,
            IEnumerable<ProjectionEventSelector> eventSelectors,
            ProjectionState initialState)
            => new(
                embedding,
                eventSelectors.Select(_ => new Projections.Store.Definition.ProjectionEventSelector(
                    _.EventType,
                    _.EventKeySelectorType,
                    _.EventKeySelectorExpression)),
                initialState);
        public EmbeddingDefinition ToStored(Store.Definition.EmbeddingDefinition definition)
            => new()
            {
                Embedding = definition.Embedding,
                InitialStateRaw = definition.InititalState,
                InitialState = BsonDocument.Parse(definition.InititalState),
                EventSelectors = definition.Events.Select(_ => new ProjectionEventSelector
                {
                    EventKeySelectorType = _.KeySelectorType,
                    EventKeySelectorExpression = _.KeySelectorExpression,
                    EventType = _.EventType,
                }).ToArray()
            };
    }
}
