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
    public class PublicEventsFetcher : AbstractEventsFromWellKnownStreamsFetcher
    {
        readonly FilterDefinitionBuilder<PublicEvent> _publicEventsFilter = Builders<PublicEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventsFetcher"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public PublicEventsFetcher(EventStoreConnection connection, ILogger logger)
            : base(new StreamId[] { StreamId.PublicEventsId })
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
                var committedEventWithPartition = await _connection.PublicEvents.Find(
                    _publicEventsFilter.Eq(_ => _.StreamPosition, streamPosition.Value))
                    .Project(_ => _.ToRuntimeStreamEvent())
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (committedEventWithPartition == default) throw new NoEventInStreamAtPosition(StreamId.PublicEventsId, streamPosition);
                return committedEventWithPartition;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<Runtime.Events.Streams.StreamEvent>> FetchRange(StreamId streamId, StreamPosition fromPostition, StreamPosition toPosition, CancellationToken cancellationToken)
        {
            try
            {
                var maxNumEvents = toPosition.Value - fromPostition.Value + 1U;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                var events = await _connection.PublicEvents.Find(
                    _publicEventsFilter.Gte(_ => _.StreamPosition, fromPostition.Value) & _publicEventsFilter.Lte(_ => _.StreamPosition, toPosition.Value))
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
                var @event = await _connection.PublicEvents.Find(
                    _publicEventsFilter.Eq(_ => _.StreamPosition, fromPosition.Value))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return @event != default ? @event.StreamPosition : uint.MaxValue;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}