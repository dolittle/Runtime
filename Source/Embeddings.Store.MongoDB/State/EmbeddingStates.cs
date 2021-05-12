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
using Dolittle.Runtime.Rudimentary;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.State
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbeddingStates" />.
    /// </summary>
    [SingletonPerTenant]
    public class EmbeddingStates : IEmbeddingStates
    {
        readonly IEmbeddings _embeddings;

        /// <summary>
        /// Initializes an instance of the <see cref="EmbeddingStates" /> class.
        /// </summary>
        /// <param name="embeddings">The embeddings repository.</param>
        public EmbeddingStates(IEmbeddings embeddings)
        {
            _embeddings = embeddings;
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
                        var deleteResult = await collection.DeleteManyAsync(Builders<Embedding>.Filter.Empty, token).ConfigureAwait(false);
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

        /// <inheritdoc/>
        public async Task<Try<EmbeddingState>> TryGet(EmbeddingId embedding, ProjectionKey key, CancellationToken token)
        {
            try
            {
                var embeddingState = await OnEmbedding(
                    embedding,
                    async collection => await collection
                                            .Find(CreateKeyFilter(key))
                                            .Project(_ => _.ContentRaw)
                                            .SingleOrDefaultAsync(token),
                    token).ConfigureAwait(false);
                return string.IsNullOrEmpty(embeddingState) ? Try<EmbeddingState>.Failed() : new EmbeddingState(embeddingState);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try<IEnumerable<(EmbeddingState State, ProjectionKey Key)>>> TryGetAll(EmbeddingId embedding, CancellationToken token)
        {
            try
            {
                var states = await OnEmbedding(
                    embedding,
                    async collection => await collection
                                            .Find(Builders<Embedding>.Filter.Empty)
                                            .Project(_ => new Tuple<EmbeddingState, ProjectionKey>(_.ContentRaw, _.Key))
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
        public async Task<bool> TryRemove(EmbeddingId embedding, ProjectionKey key, CancellationToken token)
        {
            try
            {
                return await OnEmbedding(
                    embedding,
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

        /// <inheritdoc/>
        public async Task<bool> TryReplace(EmbeddingId embedding, ProjectionKey key, EmbeddingState state, CancellationToken token)
        {
            try
            {
                return await OnEmbedding(
                    embedding,
                    async collection =>
                    {
                        var filter = CreateKeyFilter(key);
                        var updateDefinition = Builders<Embedding>
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

        async Task<TResult> OnEmbedding<TResult>(EmbeddingId embedding, Func<IMongoCollection<Embedding>, Task<TResult>> callback, CancellationToken token)
            => await callback(await _embeddings.GetStates(embedding, token).ConfigureAwait(false)).ConfigureAwait(false);

        FilterDefinition<Embedding> CreateKeyFilter(ProjectionKey key) => Builders<Embedding>.Filter.Eq(_ => _.Key, key.Value);
    }
}
