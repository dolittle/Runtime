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
        readonly FilterDefinitionBuilder<Event> _eventFilter = Builders<Event>.Filter;
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
        public async Task<StreamEvent> Fetch(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken = default)
        {
            try
            {
                var stream = streamId == StreamId.AllStreamId ? _connection.AllStream : await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var committedEventWithPartition = await stream.Find(
                    _eventFilter.Eq(_ => _.StreamPosition, streamPosition.Value))
                    .Project(_ => _.ToStreamEvent(streamId, _.Partition))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (committedEventWithPartition == default) throw new NoEventInStreamAtPosition(streamId, streamPosition);
                return committedEventWithPartition;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken = default)
        {
            if (streamId == StreamId.AllStreamId) return FindNextInAllStream(fromPosition, cancellationToken);
            else return FindNextInEventStreams(streamId, partitionId, fromPosition, cancellationToken);
        }

        async Task<StreamPosition> FindNextInAllStream(StreamPosition fromPosition, CancellationToken cancellationToken)
        {
            try
            {
                var @event = await _connection.AllStream.Find(
                    Builders<Event>.Filter.Eq(_ => _.StreamPosition, fromPosition.Value))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return @event != default ? @event.StreamPosition : uint.MaxValue;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        async Task<StreamPosition> FindNextInEventStreams(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken)
        {
            try
            {
                var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var streamEvent = await stream.Find(
                    _eventFilter.Eq(_ => _.Partition, partitionId.Value)
                    & _eventFilter.Gte(_ => _.StreamPosition, fromPosition.Value))
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