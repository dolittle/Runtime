// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Types;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsFromStreams" />.
    /// </summary>
    public class EventsFromStreamsFetcher : IFetchEventsFromStreams
    {
        readonly IEnumerable<ICanFetchEventsFromWellKnownStreams> _wellKnownStreamFetchers;
        readonly FilterDefinitionBuilder<Events.StreamEvent> _streamEventFilter = Builders<Events.StreamEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromStreamsFetcher"/> class.
        /// </summary>
        /// <param name="wellKnownStreamFetchers">The instances of <see cref="ICanFetchEventsFromWellKnownStreams" />.</param>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventsFromStreamsFetcher(
            IInstancesOf<ICanFetchEventsFromWellKnownStreams> wellKnownStreamFetchers,
            EventStoreConnection connection,
            ILogger logger)
        {
            _wellKnownStreamFetchers = wellKnownStreamFetchers;
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<Runtime.Events.Streams.StreamEvent> Fetch(StreamId stream, StreamPosition streamPosition, CancellationToken cancellationToken = default)
        {
            if (TryGetFetcher(stream, out var fetcher)) return fetcher.Fetch(stream, streamPosition, cancellationToken);
            return FetchFromStream(stream, streamPosition, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Runtime.Events.Streams.StreamEvent>> FetchRange(StreamId stream, StreamPosition fromPostition, StreamPosition toPosition, CancellationToken cancellationToken = default)
        {
            ThrowIfIllegalRange(fromPostition, toPosition);
            if (TryGetFetcher(stream, out var fetcher)) return fetcher.FetchRange(stream, fromPostition, toPosition, cancellationToken);
            return FetchRangeFromStream(stream, fromPostition, toPosition, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(StreamId stream, PartitionId partition, StreamPosition fromPosition, CancellationToken cancellationToken = default)
        {
            if (TryGetFetcher(stream, out var fetcher)) return fetcher.FindNext(stream, partition, fromPosition, cancellationToken);
            return FindNextInStream(stream, partition, fromPosition, cancellationToken);
        }

        bool TryGetFetcher(StreamId stream, out ICanFetchEventsFromWellKnownStreams fetcher)
        {
            fetcher = null;
            foreach (var instance in _wellKnownStreamFetchers)
            {
                if (instance.CanFetchFromStream(stream))
                {
                    if (fetcher != null) throw new MultipleWellKnownStreamEventFetchers(stream);
                    fetcher = instance;
                }
            }

            return fetcher != null;
        }

        async Task<Runtime.Events.Streams.StreamEvent> FetchFromStream(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken)
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

        async Task<IEnumerable<Runtime.Events.Streams.StreamEvent>> FetchRangeFromStream(StreamId streamId, StreamPosition fromPostition, StreamPosition toPosition, CancellationToken cancellationToken)
        {
            try
            {
                var maxNumEvents = toPosition.Value - fromPostition.Value + 1U;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var events = await stream.Find(
                    _streamEventFilter.Gte(_ => _.StreamPosition, fromPostition.Value) & _streamEventFilter.Lte(_ => _.StreamPosition, toPosition.Value))
                    .Limit(limit)
                    .Project(_ => _.ToRuntimeStreamEvent(streamId))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return events.ToArray();
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
                    _streamEventFilter.Eq(_ => _.Metadata.Partition, partitionId.Value)
                    & _streamEventFilter.Gte(_ => _.StreamPosition, fromPosition.Value))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return streamEvent != default ? streamEvent.StreamPosition : uint.MaxValue;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        void ThrowIfIllegalRange(StreamPosition fromPosition, StreamPosition toPosition)
        {
            if (fromPosition.Value > toPosition.Value) throw new InvalidStreamPositionRange(fromPosition, toPosition);
        }
    }
}