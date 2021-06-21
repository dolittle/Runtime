// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.State
{
    /// <summary>
    /// Represents the mongodb embedding store representation of an embedding.
    /// </summary>
    public class Embedding
    {
        /// <summary>
        /// Gets or sets the projection key of the embedding.
        /// </summary>
        [BsonId]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the bson document representation of the embedding content.
        /// </summary>
        public BsonDocument Content { get; set; }

        /// <summary>
        /// Gets or sets the raw string representation of the embedding content.
        /// </summary>
        public string ContentRaw { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the embedding currently is deleted.
        /// </summary>
        public bool IsRemoved { get; set; }

        /// <summary>
        /// Gets or sets the aggregate root version.
        /// </summary>
        public ulong Version { get; set; }
    }
}
