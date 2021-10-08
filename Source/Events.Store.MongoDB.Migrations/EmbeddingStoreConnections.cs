// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.Embeddings.Store.MongoDB;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public class EmbeddingStoreConnections : IEmbeddingStoreConnections
    {
        public Embeddings.Store.MongoDB.DatabaseConnection GetFor(EmbeddingsConfiguration configuration)
            => new (new EmbeddingStoreDatabaseConfiguration{ Instance = configuration });
    }
}