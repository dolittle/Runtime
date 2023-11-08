// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Server.Bootstrap;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB;

public class DatabaseMigrator : ICanPerformBoostrapProcedure
{
    readonly Func<TenantId, IDatabaseConnection> _getDatabase;

    public DatabaseMigrator(Func<TenantId, IDatabaseConnection> getDatabase)
    {
        _getDatabase = getDatabase;
    }

    public Task Perform() => Task.CompletedTask;

    public async Task PerformForTenant(TenantId tenant)
    {
        var db = _getDatabase(tenant).Database;
        var streams = await GetStreams(db);
        await CleanupDefaultMetadata(db, streams);
    }

    async Task CleanupDefaultMetadata(IMongoDatabase db, IList<string> streams)
    {
        foreach (var stream in streams)
        {
            var collection = db.GetCollection<BsonDocument>(stream);
            await RemoveEventHorizonDefaultMetadata(collection);
            await RemoveAggregateDefaultDefaultMetadata(collection);
        }
    }

    static async Task<IList<string>> GetStreams(IMongoDatabase db)
    {
        var collections = await (await db.ListCollectionNamesAsync()).ToListAsync();
        var streams = collections.Where(StreamIdMatcher.IsMatch).ToList();
        return streams;
    }

    /// <summary>
    /// Removes the event log default values for the EventHorizon metadata property for events not sent via the Event Horizon.
    /// </summary>
    /// <param name="collection"></param>
    Task RemoveEventHorizonDefaultMetadata<T>(IMongoCollection<T> collection)
    {
        var filter = Builders<T>.Filter.Eq("EventHorizon.FromEventHorizon", false);
        var update = Builders<T>.Update.Unset("EventHorizon");

        return collection.UpdateManyAsync(filter, update);
    }

    /// <summary>
    /// Removes the event log default values for the Aggregate metadata property for events not applied by an Aggregate.
    /// </summary>
    /// <param name="collection"></param>
    Task RemoveAggregateDefaultDefaultMetadata<T>(IMongoCollection<T> collection)
    {
        var filter = Builders<T>.Filter.Eq("Aggregate.WasAppliedByAggregate", false);
        var update = Builders<T>.Update.Unset("Aggregate");

        return collection.UpdateManyAsync(filter, update);
    }

    public int Priority => 1000;
}
