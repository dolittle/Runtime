// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsFromStreams" />.
    /// </summary>
    public class EventsFromStreamsFetcher : IFetchEventsFromStreams
    {
        readonly FilterDefinitionBuilder<MongoDB.Events.StreamEvent> _streamEventFilter = Builders<MongoDB.Events.StreamEvent>.Filter;
        readonly FilterDefinitionBuilder<Event> _eventLogFilter = Builders<Event>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromStreamsFetcher"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventsFromStreamsFetcher(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<Streams.StreamEvent> Fetch(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken = default)
        {
            if (streamId == StreamId.AllStreamId) return FetchFromEventLog(streamPosition.Value, cancellationToken);
            else return FetchFromStream(streamId, streamPosition, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken = default)
        {
            if (streamId == StreamId.AllStreamId) return FindNextInEventLog(partitionId, fromPosition.Value, cancellationToken);
            else return FindNextInStream(streamId, partitionId, fromPosition, cancellationToken);
        }

        async Task<Streams.StreamEvent> FetchFromStream(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken)
        {
            try
            {
                var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var committedEventWithPartition = await stream.Find(
                    _streamEventFilter.Eq(_ => _.StreamPosition, streamPosition.Value))
                    .Project(_ => _.ToRuntimeStreamEvent(streamId))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (committedEventWithPartition == default) throw new NoEventInStreamAtPosition(streamId, streamPosition);
                return committedEventWithPartition;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        async Task<Streams.StreamEvent> FetchFromEventLog(EventLogVersion eventLogVersion, CancellationToken cancellationToken)
        {
            try
            {
                var eventLog = _connection.EventLog;
                var committedEventWithPartition = await eventLog.Find(
                    _eventLogFilter.Eq(_ => _.EventLogVersion, eventLogVersion.Value))
                    .Project(_ => _.ToRuntimeStreamEvent())
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (committedEventWithPartition == default) throw new NoEventAtEventLogVersion(eventLogVersion);
                return committedEventWithPartition;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        async Task<StreamPosition> FindNextInEventLog(PartitionId partitionId, EventLogVersion fromPosition, CancellationToken cancellationToken)
        {
            if (partitionId != PartitionId.NotSet) return uint.MaxValue;
            try
            {
                var @event = await _connection.EventLog.Find(
                    _eventLogFilter.Eq(_ => _.EventLogVersion, fromPosition.Value))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return @event != default ? @event.EventLogVersion : uint.MaxValue;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        async Task<StreamPosition> FindNextInStream(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken)
        {
            try
            {
                var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var streamEvent = await stream.Find(
                    _streamEventFilter.Eq(_ => _.Partition, partitionId.Value)
                    & _streamEventFilter.Gte(_ => _.StreamPosition, fromPosition.Value))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return streamEvent != default ? streamEvent.StreamPosition : uint.MaxValue;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}