// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.Definition
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbeddingDefinitions" />.
    /// </summary>
    [SingletonPerTenant]
    public class EmbeddingDefinitions : IEmbeddingDefinitions
    {
        readonly IEmbeddings _embeddings;
        readonly IConvertEmbeddingDefinition _definitionConverter;

        public EmbeddingDefinitions(IEmbeddings embeddings, IConvertEmbeddingDefinition definitionConverter)
        {
            _embeddings = embeddings;
            _definitionConverter = definitionConverter;
        }

        public async Task<Try<Store.Definition.EmbeddingDefinition>> TryGet(EmbeddingId embedding, CancellationToken token)
        {
            try
            {
                return await OnDefinitions<Try<Store.Definition.EmbeddingDefinition>>(
                    async collection =>
                    {
                        var definition = await collection
                                            .Find(CreateIdFilter(embedding))
                                            .SingleOrDefaultAsync(token)
                                            .ConfigureAwait(false);
                        return definition == null
                            ? Try<Store.Definition.EmbeddingDefinition>.Failed(new EmbeddingDefinitionDoesNotExist(embedding))
                            : _definitionConverter.ToRuntime(definition);
                    },
                    token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        public async Task<bool> TryPersist(Store.Definition.EmbeddingDefinition definition, CancellationToken token)
        {
            try
            {
                return await OnDefinitions(
                    async collection =>
                    {
                        var updateResult = await collection
                                            .ReplaceOneAsync(
                                                CreateIdFilter(definition.Embedding),
                                                _definitionConverter.ToStored(definition),
                                                new ReplaceOptions { IsUpsert = true },
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
        }

        async Task<TResult> OnDefinitions<TResult>(
            Func<IMongoCollection<EmbeddingDefinition>, Task<TResult>> callback,
            CancellationToken token)
            => await callback(await _embeddings.GetDefinitions(token).ConfigureAwait(false))
                .ConfigureAwait(false);

        FilterDefinition<EmbeddingDefinition> CreateIdFilter(EmbeddingId embedding)
            => Builders<EmbeddingDefinition>.Filter.Eq(_ => _.Embedding, embedding.Value);
    }
}
