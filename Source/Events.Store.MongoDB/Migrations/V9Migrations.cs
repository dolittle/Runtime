// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations;

public interface IDbMigration
{
    public Task MigrateTenant();
}

[PerTenant]
public class V9Migrations : IDbMigration
{
    readonly IManageDatabaseMetadata _metadataManager;
    readonly IMongoDatabase _db;
    readonly ILogger<V9Migrations> _logger;
    readonly TenantId _tenantId;

    const string MigrationVersion = "V9.0.0";


    public V9Migrations(IManageDatabaseMetadata metadataManager, IDatabaseConnection db, ILogger<V9Migrations> logger, TenantId tenantId)
    {
        _metadataManager = metadataManager;
        _db = db.Database;
        _logger = logger;
        _tenantId = tenantId;
    }

    public Task MigrateTenant()
    {
        _ = Task.Run(MigrateTenantInBackground);
        return Task.CompletedTask;
    }

    async Task MigrateTenantInBackground()
    {
        var metadata = await _metadataManager.Get();

        if (metadata?.Migrations.Any(it => it.Version.Equals(MigrationVersion)) == true) return;

        _logger.LogInformation("Migrating tenant {TenantId} to {Version} in the background", _tenantId.Value, MigrationVersion);

        var before = Stopwatch.GetTimestamp();

        await MigrateEventCollections(await GetEventCollections(_db));

        if (metadata == null)
        {
            metadata = new DatabaseMetadata
            {
                CurrentVersion = "V9.0.0",
                UpdatedAt = DateTimeOffset.UtcNow
            };
        }

        metadata.Migrations.Add(new DatabaseMetadata.Migration()
        {
            Version = MigrationVersion,
            Timestamp = DateTimeOffset.UtcNow
        });
        metadata.CurrentVersion = MigrationVersion;
        metadata.UpdatedAt = DateTimeOffset.UtcNow;

        await _metadataManager.Set(metadata);

        _logger.LogInformation("Completed migration of tenant {TenantId} to {Version} in {Elapsed}", _tenantId.Value, MigrationVersion,
            Stopwatch.GetElapsedTime(before));
    }

    async Task MigrateEventCollection(string collectionName)
    {
        var start = Stopwatch.GetTimestamp();
        var eventCollection = _db.GetCollection<BsonDocument>(collectionName);

        await RemoveEventHorizonDefaultMetadata(eventCollection);
        await RemoveAggregateDefaultDefaultMetadata(eventCollection);
        var eventSourceIdsMigrated = await V6EventSourceMigrator.MigrateEventsourceId(eventCollection, _logger);
        if (eventSourceIdsMigrated > 0)
        {
            _logger.LogInformation("Migrated {EventSourceIdsMigrated} event source ids from V6 format in {Elapsed} for {CollectionName}", eventSourceIdsMigrated,
                Stopwatch.GetElapsedTime(start), collectionName);
        }
    }

    Task MigrateEventCollections(IList<string> streams)
    {
        return Task.WhenAll(streams.AsParallel().WithDegreeOfParallelism(4).Select(MigrateEventCollection));
    }

    static async Task<IList<string>> GetEventCollections(IMongoDatabase db)
    {
        var collections = await (await db.ListCollectionNamesAsync()).ToListAsync();
        var streams = collections.Where(StreamIdMatcher.IsStreamOrEventLog).ToList();
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
}
