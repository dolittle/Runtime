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
                                            .SingleOrDefaultAsync(token)
                                            .ConfigureAwait(false),
                    token).ConfigureAwait(false);

                return embedding == default
                    ? Try<EmbeddingState>.Failed(new EmbeddingStateDoesNotExist(embeddingId, key))
                    : new EmbeddingState(embedding.Content, embedding.Version, embedding.IsRemoved);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <inheritdoc/>
        /// <remark>
        /// Returns all of the stored embedding states, even ones that have been marked as removed.
        /// <remark/>
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
                                            .Project(_ => Tuple.Create<EmbeddingState, ProjectionKey>(
                                                new EmbeddingState(_.Content, _.Version, _.IsRemoved), _.Key))
                                            .ToListAsync(token)
                                            .ConfigureAwait(false),
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
                        var markDefinition = Builders<Embedding>
                            .Update
                            .Set(_ => _.IsRemoved, true)
                            .Set(_ => _.Version, version.Value);
                        var markResult = await collection
                            .UpdateOneAsync(
                                CreateKeyFilter(key),
                                markDefinition,
                                new UpdateOptions { IsUpsert = true },
                                token)
                            .ConfigureAwait(false);
                        return markResult.IsAcknowledged;
                    },
                    token).ConfigureAwait(false);
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
                        var updateDefinition = Builders<Embedding>
                                                .Update
                                                .Set(_ => _.Content, state.State.Value)
                                                .Set(_ => _.Version, state.Version.Value)
                                                .Set(_ => _.IsRemoved, false);
                        var updateResult = await collection
                            .UpdateOneAsync(
                                CreateKeyFilter(key),
                                updateDefinition,
                                new UpdateOptions { IsUpsert = true },
                                token)
                            .ConfigureAwait(false);
                        return updateResult.IsAcknowledged;
                    },
                    token).ConfigureAwait(false);
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

        /// <inheritdoc/>
        public async Task<Try> TryDrop(EmbeddingId embedding, CancellationToken token)
        {
            try
            {
                await OnEmbedding(
                    embedding,
                    async collection =>
                    {
                        var deleteResult = await collection
                            .DeleteManyAsync(Builders<Embedding>.Filter.Empty, token)
                            .ConfigureAwait(false);
                        return deleteResult.IsAcknowledged;
                    },
                    token).ConfigureAwait(false);
                return Try.Succeeded();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        async Task<TResult> OnEmbedding<TResult>(
            EmbeddingId embedding,
            Func<IMongoCollection<Embedding>, Task<TResult>> callback,
            CancellationToken token)
            => await callback(await _embeddings.GetStates(embedding, token).ConfigureAwait(false))
                .ConfigureAwait(false);

        FilterDefinition<Embedding> CreateKeyFilter(ProjectionKey key) =>
            Builders<Embedding>.Filter.Eq(_ => _.Key, key.Value);
    }
}
