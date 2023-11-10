// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Scoping;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations;

public interface IManageDatabaseMetadata
{
    public Task<DatabaseMetadata?> Get();
    public Task Set(DatabaseMetadata metadata);
}

[PerTenant]
public class MetadataRepository : IManageDatabaseMetadata
{
    const string CollectionName = "metadata";
    const string MetadataId = "database";


    IMongoCollection<DatabaseMetadata> _collection;

    public MetadataRepository(IDatabaseConnection db)
    {
        _collection = db.Database.GetCollection<DatabaseMetadata>(CollectionName);
    }

    public async Task<DatabaseMetadata?> Get() => await _collection.Find(Builders<DatabaseMetadata>.Filter.Eq(it => it.Id, MetadataId))
        .FirstOrDefaultAsync();

    public Task Set(DatabaseMetadata metadata) => _collection.ReplaceOneAsync(Builders<DatabaseMetadata>.Filter.Eq(it => it.Id, MetadataId), metadata,
        new ReplaceOptions { IsUpsert = true });
}
