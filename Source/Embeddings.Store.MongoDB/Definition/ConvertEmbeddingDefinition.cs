// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;
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
            IEnumerable<EmbeddingEventSelector> eventSelectors,
            Store.State.EmbeddingState initialState)
            => new(
                embedding,
                eventSelectors.Select(_ => new Store.Definition.EmbeddingEventSelector(
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
                EventSelectors = definition.Events.Select(_ => new EmbeddingEventSelector
                {
                    EventKeySelectorType = _.KeySelectorType,
                    EventKeySelectorExpression = _.KeySelectorExpression,
                    EventType = _.EventType,
                }).ToArray()
            };
    }
}
