// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.Embeddings.Store.MongoDB;
using Dolittle.Runtime.ResourceTypes.Configuration;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    public class EmbeddingStoreDatabaseConfiguration : IConfigurationFor<EmbeddingsConfiguration>
    {
        public EmbeddingsConfiguration Instance { get; init; }
    }
}