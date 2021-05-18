// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.State
{
    public class Embedding
    {
        [BsonId]
        public string Key { get; set; }
        public BsonDocument Content { get; set; }
        public string ContentRaw { get; set; }
        public bool IsRemoved { get; set; }
        public ulong Version { get; set; }
    }
}