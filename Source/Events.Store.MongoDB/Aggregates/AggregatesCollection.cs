// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Lifecycle;
using Microsoft.Extension.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates
{
    /// <summary>
    /// Represents an implementation of <see cref="IAggregatesCollection" />.
    /// </summary>
    [SingletonPerTenant]
    public class AggregatesCollection : EventStoreConnection, IAggregatesCollection
    {
        const string CollectionName = "aggregates";

        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatesCollection"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="DatabaseConnection" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public AggregatesCollection(DatabaseConnection connection, ILogger<AggregatesCollection> logger)
            : base(connection)
        {
            _logger = logger;
            Aggregates = connection.Database.GetCollection<AggregateRoot>(CollectionName);
            CreateCollectionsAndIndexesForAggregates();
        }

        /// <inheritdoc/>
        public IMongoCollection<AggregateRoot> Aggregates { get; }

        void CreateCollectionsAndIndexesForAggregates()
        {
            _logger.Trace("Creating indexes for {CollectionName}' collection in Event Store", CollectionName);
            Aggregates.Indexes.CreateOne(new CreateIndexModel<AggregateRoot>(
                Builders<AggregateRoot>.IndexKeys
                    .Ascending(_ => _.EventSource)
                    .Ascending(_ => _.AggregateType),
                new CreateIndexOptions { Unique = true }));
        }
    }
}
