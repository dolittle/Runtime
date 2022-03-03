// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates;

/// <summary>
/// Represents an implementation of <see cref="IAggregatesCollection" />.
/// </summary>
[Singleton, PerTenant]
public class AggregatesCollection : EventStoreConnection, IAggregatesCollection
{
    const string CollectionName = "aggregates";

    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregatesCollection"/> class.
    /// </summary>
    /// <param name="connection">The <see cref="IDatabaseConnection" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public AggregatesCollection(IDatabaseConnection connection, ILogger logger)
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
        Log.CreatingIndexesFor(_logger, CollectionName);
        Aggregates.Indexes.CreateOne(new CreateIndexModel<AggregateRoot>(
            Builders<AggregateRoot>.IndexKeys
                .Ascending(_ => _.EventSource)
                .Ascending(_ => _.AggregateType),
            new CreateIndexOptions { Unique = true }));
    }
}
