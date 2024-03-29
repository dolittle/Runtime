// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon;

/// <summary>
/// Represents an implementation of <see cref="ISubscriptionStateCollections" />.
/// </summary>
[Singleton, PerTenant]
public class SubscriptionStateCollections : EventStoreConnection, ISubscriptionStateCollections
{
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionStateCollections"/> class.
    /// </summary>
    /// <param name="connection">The <see cref="IDatabaseConnection" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public SubscriptionStateCollections(IDatabaseConnection connection, ILogger logger)
        : base(connection)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IMongoCollection<MongoDB.Processing.Streams.EventHorizon.SubscriptionState>> Get(ScopeId scopeId, CancellationToken token)
    {
        var collection = Database.GetCollection<MongoDB.Processing.Streams.EventHorizon.SubscriptionState>(CollectionNameForScopedSubscriptionStates(scopeId));
        await CreateCollectionsAndIndexesForSubscriptionStatesAsync(collection, token).ConfigureAwait(false);
        return collection;
    }

    static string CollectionNameForScopedSubscriptionStates(ScopeId scope) => $"x-{scope.Value}-subscription-states";

    async Task CreateCollectionsAndIndexesForSubscriptionStatesAsync(
        IMongoCollection<MongoDB.Processing.Streams.EventHorizon.SubscriptionState> subscriptionStates,
        CancellationToken cancellationToken)
    {
        Log.CreatingIndexesFor(_logger, subscriptionStates.CollectionNamespace.CollectionName);
        await subscriptionStates.Indexes.CreateOneAsync(
            new CreateIndexModel<MongoDB.Processing.Streams.EventHorizon.SubscriptionState>(
                Builders<MongoDB.Processing.Streams.EventHorizon.SubscriptionState>.IndexKeys
                    .Ascending(_ => _.Microservice)
                    .Ascending(_ => _.Tenant)
                    .Ascending(_ => _.Stream)
                    .Ascending(_ => _.Partition),
                new CreateIndexOptions { Unique = true }),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
