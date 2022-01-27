// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IProjectionCopyStore"/> that persists copies in a MongoDB collection.
/// </summary>
[SingletonPerTenant]
public class MongoDBProjectionCopyStore : IProjectionCopyStore
{
    readonly IProjectionCopiesStorage _storage;
    readonly IProjectionConverter _converter;

    public MongoDBProjectionCopyStore(IProjectionCopiesStorage storage, IProjectionConverter converter)
    {
        _storage = storage;
        _converter = converter;
    }

    /// <inheritdoc />
    public bool ShouldPersistFor(ProjectionDefinition projection)
        => projection.Copies.MongoDB.ShouldCopyToMongoDB
           && projection.Copies.MongoDB.Collection != CollectionName.NotSet;

    /// <inheritdoc />
    public async Task<bool> TryReplace(ProjectionDefinition projection, ProjectionKey key, ProjectionState state, CancellationToken token)
    {
        try
        {
            var document = _converter.Convert(state, projection.Copies.MongoDB.Conversions);
            AddKeyTo(document, key);

            var collection = GetCollectionFor(projection);
            var filter = GetFilterFor(key);

            var replaceResult = await collection.ReplaceOneAsync(filter, document, new ReplaceOptions {IsUpsert = true}, token).ConfigureAwait(false);
            return replaceResult.IsAcknowledged;
        }
        catch (MongoWaitQueueFullException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> TryRemove(ProjectionDefinition projection, ProjectionKey key, CancellationToken token)
    {
        try
        {
            var collection = GetCollectionFor(projection);
            var filter = GetFilterFor(key);

            var deleteResult = await collection.DeleteOneAsync(filter, token).ConfigureAwait(false);
            return deleteResult.IsAcknowledged;
        }
        catch (MongoWaitQueueFullException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> TryDrop(ProjectionDefinition projection, CancellationToken token)
    {
        if (!projection.Copies.MongoDB.ShouldCopyToMongoDB)
        {
            return true;
        }
        
        try
        {
            var collection = GetCollectionNameFor(projection);
            await _storage.Database.DropCollectionAsync(collection, token).ConfigureAwait(false);
            return true;
        }
        catch (MongoWaitQueueFullException)
        {
            return false;
        }
    }

    IMongoCollection<BsonDocument> GetCollectionFor(ProjectionDefinition projection)
        => _storage.Database.GetCollection<BsonDocument>(GetCollectionNameFor(projection));

    static CollectionName GetCollectionNameFor(ProjectionDefinition projection)
        => projection.Copies.MongoDB.Collection;
    
    static FilterDefinition<BsonDocument> GetFilterFor(ProjectionKey key)
        => Builders<BsonDocument>.Filter.Eq("_id", key.Value);

    static void AddKeyTo(BsonDocument document, ProjectionKey key)
        => document.Set("_id", key.Value);
}
