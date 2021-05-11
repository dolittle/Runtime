// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.Definition
{
    /// <summary>
    /// Represents the persisted definition of a embedding.
    /// </summary>
    public class EmbeddingDefinition
    {
        /// <summary>
        /// Gets or sets the embedding id.
        /// </summary>
        [BsonId]
        public Guid Embedding { get; set; }

        /// <summary>
        /// Gets or sets the initial state.
        /// </summary>
        public BsonDocument InitialState { get; set; }

        /// <summary>
        /// Gets or sets the raw initial state.
        /// </summary>
        public string InitialStateRaw { get; set; }

        /// <summary>
        /// Gets or sets the embedding event selectors.
        /// </summary>
        public EmbeddingEventSelector[] EventSelectors { get; set; }
    }
}
