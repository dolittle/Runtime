// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Filters;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterDefinitionRepository" />.
    /// </summary>
    public class FilterDefinitionRepository : IFilterDefinitionRepository
    {
        readonly FilterDefinitionBuilder<FilterDefinition> _streamProcessorFilter = Builders<FilterDefinition>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterDefinitionRepository"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public FilterDefinitionRepository(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IFilterDefinition> GetPersistedFilter(IFilterDefinition filterDefinition, CancellationToken cancellationToken)
        {
            try
            {
                var persistedDefinition = await _connection.FilterDefinitions
                    .Find(
                        _streamProcessorFilter.Eq(_ => _.FilterId, filterDefinition.TargetStream.Value))
                        .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return persistedDefinition.AsRuntimeRepresentation();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task PersistFilter(IFilterDefinition filterDefinition, CancellationToken cancellationToken)
        {
            try
            {
                if (!filterDefinition.IsPersistable) return;
                var document = filterDefinition switch
                    {
                        TypeFilterWithEventSourcePartitionDefinition definition => definition.ToStoreRepresentation(),
                        _ => new FilterDefinition(filterDefinition.TargetStream, filterDefinition.SourceStream)
                    };

                await _connection.FilterDefinitions
                    .ReplaceOneAsync(
                        _streamProcessorFilter.Eq(_ => _.FilterId, filterDefinition.TargetStream.Value),
                        document,
                        new ReplaceOptions { IsUpsert = true }).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}