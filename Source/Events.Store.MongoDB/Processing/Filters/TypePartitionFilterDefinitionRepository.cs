// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterDefinitionRepositoryFor{T}" /> for <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
    /// </summary>
    public class TypePartitionFilterDefinitionRepository : IFilterDefinitionRepositoryFor<TypeFilterWithEventSourcePartitionDefinition>
    {
        readonly FilterDefinitionBuilder<TypePartitionFilterDefinition> _streamProcessorFilter = Builders<TypePartitionFilterDefinition>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypePartitionFilterDefinitionRepository"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public TypePartitionFilterDefinitionRepository(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<TypeFilterWithEventSourcePartitionDefinition> GetPersistedFilter(TypeFilterWithEventSourcePartitionDefinition filterDefinition, CancellationToken cancellationToken)
        {
            try
            {
                var persistedDefinition = await _connection.TypePartitionFilterDefinitions
                    .Find(
                        _streamProcessorFilter.Eq(_ => _.TargetStream, filterDefinition.TargetStream.Value))
                        .Limit(1)
                        .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return persistedDefinition.ToRuntimeRepresentation();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task PersistFilter(TypeFilterWithEventSourcePartitionDefinition filterDefinition, CancellationToken cancellationToken)
        {
            try
            {
                var document = filterDefinition.ToStoreRepresentation();
                await _connection.TypePartitionFilterDefinitions
                    .ReplaceOneAsync(
                        _streamProcessorFilter.Eq(_ => _.TargetStream, filterDefinition.TargetStream.Value),
                        document,
                        new ReplaceOptions { IsUpsert = true }).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task RemovePersistedFilter(StreamId targetStream, CancellationToken cancellationToken)
        {
            try
            {
                await _connection.TypePartitionFilterDefinitions
                    .DeleteOneAsync(
                        _streamProcessorFilter.Eq(_ => _.TargetStream, targetStream.Value)).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}