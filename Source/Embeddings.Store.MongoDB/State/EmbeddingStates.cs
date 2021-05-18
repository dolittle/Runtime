// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.State
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionStates" />.
    /// </summary>
    [SingletonPerTenant]
    public class EmbeddingStates : IEmbeddingStates
    {
        readonly IEmbeddings _embeddings;

        /// <summary>
        /// Initializes an instance of the <see cref="ProjectionStates" /> class.
        /// </summary>
        /// <param name="embeddings">The embeddings repository.</param>
        public EmbeddingStates(IEmbeddings embeddings)
        {
            _embeddings = embeddings;
        }

        /// <inheritdoc/>
        public async Task<Try<EmbeddingState>> TryGet(
            EmbeddingId embeddingId,
            ProjectionKey key,
            CancellationToken token)
        {
            try
            {
                var embedding = await OnEmbedding(
                    embeddingId,
                    async collection => await collection
                                            .Find(CreateKeyFilter(key))
                                            .SingleOrDefaultAsync(token),
                    token).ConfigureAwait(false);
                if (embedding.IsRemoved)
                {
                    return Try<EmbeddingState>.Failed();
                }
                return string.IsNullOrEmpty(embedding.ContentRaw)
                    ? Try<EmbeddingState>.Failed()
                    : new EmbeddingState(embedding.ContentRaw, embedding.Version);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try<IEnumerable<(EmbeddingState State, ProjectionKey Key)>>> TryGetAll(
            EmbeddingId embedding,
            CancellationToken token)
        {
            try
            {
                var states = await OnEmbedding(
                    embedding,
                    async collection => await collection
                                            .Find(Builders<Embedding>.Filter.Empty)
                                            .Project(_ => new Tuple<EmbeddingState, ProjectionKey>(new EmbeddingState(_.ContentRaw, _.Version), _.Key))
                                            .ToListAsync(token),
                    token).ConfigureAwait(false);
                var result = states.Select(_ =>
                {
                    return (_.Item1, _.Item2);
                });

                return Try<IEnumerable<(EmbeddingState State, ProjectionKey Key)>>.Succeeded(result);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try<bool>> TryMarkAsRemove(
            EmbeddingId embedding,
            ProjectionKey key,
            AggregateRootVersion version,
            CancellationToken token)
        {
            try
            {
                return await OnEmbedding(
                    embedding,
                    async collection =>
                    {
                        var deleteResult = await collection.DeleteOneAsync(
                            Builders<Embedding>.Filter.And(CreateKeyFilter(key), CreateAggregateVersionFilter(version)),
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

        /// <inheritdoc/>
        public async Task<Try<bool>> TryReplace(
            EmbeddingId embedding,
            ProjectionKey key,
            EmbeddingState state,
            CancellationToken token)
        {
            try
            {
                return await OnEmbedding(
                    embedding,
                    async collection =>
                    {
                        // @joel do we need to check the version of the aggregate root here so that it has incremented?
                        var updateDefinition = Builders<Embedding>
                                                .Update
                                                .Set(_ => _.Content, BsonDocument.Parse(state.State))
                                                .Set(_ => _.ContentRaw, state.State)
                                                .Set(_ => _.Version, state.Version.Value);
                        var updateResult = await collection.UpdateOneAsync(
                            CreateKeyFilter(key),
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

        /// <inheritdoc/>
        public async Task<bool> TryDrop(EmbeddingId embedding, CancellationToken token)
        {
            try
            {
                await OnEmbedding(
                    embedding,
                    async collection =>
                    {
                        var deleteResult = await collection.DeleteManyAsync(Builders<Embedding>.Filter.Empty, token)
                            .ConfigureAwait(false);
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

        async Task<TResult> OnEmbedding<TResult>(
            EmbeddingId embedding,
            Func<IMongoCollection<Embedding>, Task<TResult>> callback,
            CancellationToken token)
            => await callback(await _embeddings.GetStates(embedding, token).ConfigureAwait(false)).ConfigureAwait(false);

        FilterDefinition<Embedding> CreateKeyFilter(ProjectionKey key) =>
            Builders<Embedding>.Filter.Eq(_ => _.Key, key.Value);
        FilterDefinition<Embedding> CreateAggregateVersionFilter(AggregateRootVersion version) =>
            Builders<Embedding>.Filter.Eq(_ => _.Version, version.Value);
    }
}
