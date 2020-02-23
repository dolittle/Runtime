// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="ITypePartitionFilterRegistry" />.
    /// </summary>
    public class TypePartitionFilterRegistry : ITypePartitionFilterRegistry
    {
        readonly FilterDefinitionBuilder<TypePartitionFilterDefinition> _streamProcessorFilter = Builders<TypePartitionFilterDefinition>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypePartitionFilterRegistry"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public TypePartitionFilterRegistry(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Register(StreamId targetStream, StreamId sourceStream, TypeFilterWithEventSourcePartitionDefinition definition)
        {
            try
            {
                var document = TypePartitionFilterDefinition.FromDefinition(definition, targetStream, sourceStream);
                await _connection.TypePartitionFilterDefinitions
                    .ReplaceOneAsync(
                        _streamProcessorFilter.Eq(_ => _.Id, document.Id),
                        document,
                        new ReplaceOptions { IsUpsert = true }).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task Remove(StreamId targetStream, StreamId sourceStream)
        {
            try
            {
                var id = new TypePartitionFilterDefinitionId { SourceStream = sourceStream, TargetStream = targetStream };
                await _connection.TypePartitionFilterDefinitions
                    .DeleteOneAsync(
                        _streamProcessorFilter.Eq(_ => _.Id, id)).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task Validate(StreamId targetStream, StreamId sourceStream, TypeFilterWithEventSourcePartitionDefinition definition)
        {
            try
            {
                var document = TypePartitionFilterDefinition.FromDefinition(definition, targetStream, sourceStream);
                var storedDefinition = await _connection.TypePartitionFilterDefinitions
                    .Find(
                        _streamProcessorFilter.Eq(_ => _.Id, document.Id)).FirstOrDefaultAsync().ConfigureAwait(false);
                if (storedDefinition != default && !document.HasSameDefinitionAs(storedDefinition)) throw new TypeFilterDefinitionDoesNotMatchPersistedDefinition(targetStream, sourceStream);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}