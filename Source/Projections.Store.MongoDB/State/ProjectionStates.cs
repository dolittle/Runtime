// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Projections.Store.MongoDB.State
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionStates" />.
    /// </summary>
    [SingletonPerTenant]
    public class ProjectionStates : IProjectionStates
    {
        readonly IProjections _projections;

        public ProjectionStates(IProjections projections)
        {
            _projections = projections;
        }
        public async Task<bool> TryDrop(ProjectionId projection, ScopeId scope, CancellationToken token)
        {
            try
            {
                await OnProjection(
                    projection,
                    scope,
                    async collection =>
                    {
                        var deleteResult = await collection.DeleteManyAsync(Builders<Projection>.Filter.Empty, token).ConfigureAwait(false);
                        return deleteResult.IsAcknowledged;
                    },
                    token).ConfigureAwait(false);
                return true;
            }
            catch (MongoWaitQueueFullException)
            {
                return false;
            }
        }
        public async Task<Try<ProjectionState>> TryGet(ProjectionId projection, ScopeId scope, ProjectionKey key, CancellationToken token)
        {
            try
            {
                var projectionState = await OnProjection(
                    projection,
                    scope,
                    async collection => await collection
                                            .Find(CreateKeyFilter(key))
                                            .Project(_ => _.ContentRaw)
                                            .SingleAsync(token),
                    token).ConfigureAwait(false);
                return new ProjectionState(projectionState);
            }
            catch (MongoWaitQueueFullException)
            {
                return false;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        public async Task<bool> TryRemove(ProjectionId projection, ScopeId scope, ProjectionKey key, CancellationToken token)
        {
            try
            {
                return await OnProjection(
                    projection,
                    scope,
                    async collection =>
                    {
                        var deleteResult = await collection.DeleteOneAsync(
                            CreateKeyFilter(key),
                            token).ConfigureAwait(false);
                        return deleteResult.IsAcknowledged;
                    },
                    token).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException)
            {
                return false;
            }
        }
        public async Task<bool> TryReplace(ProjectionId projection, ScopeId scope, ProjectionKey key, ProjectionState state, CancellationToken token)
        {
            try
            {
                return await OnProjection(
                    projection,
                    scope,
                    async collection =>
                    {
                        var filter = CreateKeyFilter(key);
                        var updateDefinition = Builders<Projection>
                                                .Update
                                                .Set(_ => _.Content, BsonDocument.Parse(state.Value))
                                                .Set(_ => _.ContentRaw, state.Value);
                        var updateResult = await collection.UpdateOneAsync(
                            filter,
                            updateDefinition,
                            new UpdateOptions { IsUpsert = true },
                            token).ConfigureAwait(false);
                        return updateResult.IsAcknowledged;
                    },
                    token).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException)
            {
                return false;
            }
        }

        async Task<TResult> OnProjection<TResult>(ProjectionId projection, ScopeId scope, Func<IMongoCollection<Projection>, Task<TResult>> callback, CancellationToken token)
            => await callback(await _projections.Get(scope, projection, token).ConfigureAwait(false)).ConfigureAwait(false);

        FilterDefinition<Projection> CreateKeyFilter(ProjectionKey key) => Builders<Projection>.Filter.Eq(_ => _.Key, key.Value);

    }
}