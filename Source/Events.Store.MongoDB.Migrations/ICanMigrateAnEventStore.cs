// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store.MongoDB;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations
{
    /// <summary>
    /// Defines a system that can migrate an Event Store.
    /// </summary>
    public interface ICanMigrateAnEventStore
    {
        Task<Try> Migrate(EventStoreConfiguration eventStoreConfiguration);
        Task<Try> Migrate(EventStoreConfiguration eventStoreConfiguration, EmbeddingsConfiguration embeddingStoreConnections);
    }
}
