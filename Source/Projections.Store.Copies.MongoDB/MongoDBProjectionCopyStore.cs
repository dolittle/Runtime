// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IProjectionCopyStore"/> that persists copies in a MongoDB collection.
/// </summary>
[Singleton, PerTenant]
public class MongoDBProjectionCopyStore : IProjectionCopyStore
{
    const string ProjectionDocumentKeyProperty = "_dolittle_projection_key";
    
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
        ThrowIfShouldNotPersistFor(projection);
        
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
        ThrowIfShouldNotPersistFor(projection);
        
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
        ThrowIfShouldNotPersistFor(projection);
        
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

    void ThrowIfShouldNotPersistFor(ProjectionDefinition projection)
    {
        if (!ShouldPersistFor(projection))
        {
            throw new ProjectionShouldNotBeCopiedToMongoDB(projection);
        }
    }

    IMongoCollection<BsonDocument> GetCollectionFor(ProjectionDefinition projection)
        => _storage.Database.GetCollection<BsonDocument>(GetCollectionNameFor(projection));

    static CollectionName GetCollectionNameFor(ProjectionDefinition projection)
        => projection.Copies.MongoDB.Collection;
    
    static FilterDefinition<BsonDocument> GetFilterFor(ProjectionKey key)
        => Builders<BsonDocument>.Filter.Eq(ProjectionDocumentKeyProperty, key.Value);

    static void AddKeyTo(BsonDocument document, ProjectionKey key)
        => document.Set(ProjectionDocumentKeyProperty, key.Value);
}
