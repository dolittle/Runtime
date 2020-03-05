// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractEventsFromWellKnownStreamsFetcher" /> that can fetch events from the event log.
    /// </summary>
    public class EventFromEventLogFetcher : AbstractEventsFromWellKnownStreamsFetcher
    {
        readonly FilterDefinitionBuilder<Event> _eventLogFilter = Builders<Event>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFromEventLogFetcher"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventFromEventLogFetcher(EventStoreConnection connection, ILogger logger)
            : base(new StreamId[] { StreamId.AllStreamId })
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<Runtime.Events.Streams.StreamEvent> Fetch(StreamId stream, StreamPosition streamPosition, CancellationToken cancellationToken = default)
        {
            if (!CanFetchFromStream(stream)) throw new EventsFromWellKnownStreamsFetcherCannotFetchFromStream(this, stream);
            try
            {
                var committedEventWithPartition = await _connection.EventLog.Find(
                    _eventLogFilter.Eq(_ => _.EventLogSequenceNumber, streamPosition.Value))
                    .Project(_ => _.ToRuntimeStreamEvent())
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (committedEventWithPartition == default) throw new NoEventInStreamAtPosition(StreamId.AllStreamId, streamPosition);
                return committedEventWithPartition;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<Runtime.Events.Streams.StreamEvent>> FetchRange(StreamId stream, StreamPositionRange range, CancellationToken cancellationToken = default)
        {
            if (!CanFetchFromStream(stream)) throw new EventsFromWellKnownStreamsFetcherCannotFetchFromStream(this, stream);
            try
            {
                var maxNumEvents = range.To.Value - range.From.Value + 1U;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                var events = await _connection.EventLog.Find(
                    _eventLogFilter.Gte(_ => _.EventLogSequenceNumber, range.From.Value) & _eventLogFilter.Lte(_ => _.EventLogSequenceNumber, range.To.Value))
                    .Limit(limit)
                    .Project(_ => _.ToRuntimeStreamEvent())
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return events.ToArray();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public override async Task<StreamPosition> FindNext(StreamId stream, PartitionId partition, StreamPosition fromPosition, CancellationToken cancellationToken = default)
        {
            if (!CanFetchFromStream(stream)) throw new EventsFromWellKnownStreamsFetcherCannotFetchFromStream(this, stream);
            if (partition != PartitionId.NotSet) return uint.MaxValue;
            try
            {
                var @event = await _connection.EventLog.Find(
                    _eventLogFilter.Eq(_ => _.EventLogSequenceNumber, fromPosition.Value))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return @event != default ? @event.EventLogSequenceNumber : uint.MaxValue;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}