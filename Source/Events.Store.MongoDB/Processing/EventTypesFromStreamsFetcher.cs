// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventTypesFromStreams" />.
    /// </summary>
    public class EventTypesFromStreamsFetcher : IFetchEventTypesFromStreams
    {
        readonly FilterDefinitionBuilder<Event> _streamEventFilter = Builders<Event>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypesFromStreamsFetcher"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventTypesFromStreamsFetcher(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Artifact>> FetchTypesInRange(StreamId streamId, StreamPosition fromPosition, StreamPosition toPosition, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidRange(fromPosition, toPosition);
            try
            {
                using var session = await _connection.MongoClient.StartSessionAsync().ConfigureAwait(false);
                return await session.WithTransactionAsync(
                    async (transaction, cancellationToken) =>
                    {
                        var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                        var eventTypes = await stream
                            .Find(transaction, _streamEventFilter.Gte(_ => _.StreamPosition, fromPosition.Value) & _streamEventFilter.Lte(_ => _.StreamPosition, toPosition.Value))
                            .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                            .ToListAsync(cancellationToken).ConfigureAwait(false);
                        return new HashSet<Artifact>(eventTypes);
                    },
                    null,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Artifact>> FetchTypesInRangeAndPartition(StreamId streamId, PartitionId partition, StreamPosition fromPosition, StreamPosition toPosition, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidRange(fromPosition, toPosition);
            try
            {
                using var session = await _connection.MongoClient.StartSessionAsync().ConfigureAwait(false);
                return await session.WithTransactionAsync(
                    async (transaction, cancellationToken) =>
                    {
                        var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                        var eventTypes = await stream
                            .Find(transaction, _streamEventFilter.Eq(_ => _.Partition, partition.Value) & _streamEventFilter.Gte(_ => _.StreamPosition, fromPosition.Value) & _streamEventFilter.Lte(_ => _.StreamPosition, toPosition.Value))
                            .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                            .ToListAsync(cancellationToken).ConfigureAwait(false);
                        return new HashSet<Artifact>(eventTypes);
                    },
                    null,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        void ThrowIfInvalidRange(StreamPosition fromPosition, StreamPosition toPosition)
        {
            if (fromPosition.Value > toPosition.Value) throw new InvalidStreamPositionRange(fromPosition, toPosition);
        }
    }
}