// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventTypesFromStreams" />.
    /// </summary>
    public class EventTypesFromStreamsFetcher : IFetchEventTypesFromStreams
    {
        readonly FilterDefinitionBuilder<MongoDB.Events.StreamEvent> _streamEventFilter = Builders<MongoDB.Events.StreamEvent>.Filter;
        readonly FilterDefinitionBuilder<Event> _eventLogFilter = Builders<Event>.Filter;
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
        public Task<IEnumerable<Artifact>> FetchTypesInRange(StreamId streamId, StreamPosition fromPosition, StreamPosition toPosition, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidRange(fromPosition, toPosition);
            if (streamId == StreamId.AllStreamId) return FetchTypesInRangeFromEventLog(fromPosition.Value, toPosition.Value, cancellationToken);
            return FetchTypesInRangeFromStream(streamId, fromPosition, toPosition, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Artifact>> FetchTypesInRangeAndPartition(StreamId streamId, PartitionId partition, StreamPosition fromPosition, StreamPosition toPosition, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidRange(fromPosition, toPosition);
            if (streamId == StreamId.AllStreamId) return FetchTypesInRangeAndPartitionFromEventLog(partition, fromPosition.Value, toPosition.Value, cancellationToken);
            return FetchTypesInRangeAndPartitionFromStream(streamId, partition, fromPosition, toPosition, cancellationToken);
        }

        async Task<IEnumerable<Artifact>> FetchTypesInRangeFromEventLog(EventLogVersion fromPosition, EventLogVersion toPosition, CancellationToken cancellationToken)
        {
            try
            {
                var eventLog = _connection.EventLog;
                var eventTypes = await eventLog
                    .Find(_eventLogFilter.Gte(_ => _.EventLogVersion, fromPosition.Value) & _eventLogFilter.Lte(_ => _.EventLogVersion, toPosition.Value))
                    .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return new HashSet<Artifact>(eventTypes);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        async Task<IEnumerable<Artifact>> FetchTypesInRangeAndPartitionFromEventLog(PartitionId partition, EventLogVersion fromPosition, EventLogVersion toPosition, CancellationToken cancellationToken)
        {
            if (partition != PartitionId.NotSet) return Enumerable.Empty<Artifact>();
            try
            {
                var eventLog = _connection.EventLog;
                var eventTypes = await eventLog
                    .Find(_eventLogFilter.Gte(_ => _.EventLogVersion, fromPosition.Value) & _eventLogFilter.Lte(_ => _.EventLogVersion, toPosition.Value))
                    .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return new HashSet<Artifact>(eventTypes);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        async Task<IEnumerable<Artifact>> FetchTypesInRangeFromStream(StreamId streamId, StreamPosition fromPosition, StreamPosition toPosition, CancellationToken cancellationToken)
        {
            try
            {
                var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var eventTypes = await stream
                    .Find(_streamEventFilter.Gte(_ => _.StreamPosition, fromPosition.Value) & _streamEventFilter.Lte(_ => _.StreamPosition, toPosition.Value))
                    .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return new HashSet<Artifact>(eventTypes);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        async Task<IEnumerable<Artifact>> FetchTypesInRangeAndPartitionFromStream(StreamId streamId, PartitionId partition, StreamPosition fromPosition, StreamPosition toPosition, CancellationToken cancellationToken)
        {
            try
            {
                var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var eventTypes = await stream
                    .Find(_streamEventFilter.Eq(_ => _.Partition, partition.Value) & _streamEventFilter.Gte(_ => _.StreamPosition, fromPosition.Value) & _streamEventFilter.Lte(_ => _.StreamPosition, toPosition.Value))
                    .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return new HashSet<Artifact>(eventTypes);
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