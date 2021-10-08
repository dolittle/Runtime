// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.Embeddings.Store.MongoDB;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public interface IEmbeddingStoreConnections
    {
        Embeddings.Store.MongoDB.DatabaseConnection GetFor(EmbeddingsConfiguration configuration);
    }
}