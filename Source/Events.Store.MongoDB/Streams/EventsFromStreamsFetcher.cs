// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
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

        #nullable enable
        /// <summary>
        /// Fetches an event that has a sequence number equal the given stream position.
        /// </summary>
        /// <typeparam name="TEvent">The type of the stored event.</typeparam>
        /// <param name="stream">The <see cref="IMongoCollection{TDocument}" />.</param>
        /// <param name="filter">The <see cref="FilterDefinitionBuilder{TDocument}" />.</param>
        /// <param name="sequenceNumberExpression">The <see cref="Expression{T}" /> for getting the sequence number from the stored event.</param>
        /// <param name="projection">The <see cref="ProjectionDefinition{TSource, TProjection}" /> for projecting the stored event to a <see cref="Runtime.Events.Store.Streams.StreamEvent" />.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The fetched event or null.</returns>
        public static async Task<Runtime.Events.Store.Streams.StreamEvent?> Fetch<TEvent>(
            IMongoCollection<TEvent> stream,
            FilterDefinitionBuilder<TEvent> filter,
            Expression<Func<TEvent, ulong>> sequenceNumberExpression,
            ProjectionDefinition<TEvent, Runtime.Events.Store.Streams.StreamEvent> projection,
            StreamPosition streamPosition,
            CancellationToken cancellationToken)
            where TEvent : class
        {
            try
            {
                return await stream.Find(
                    filter.Eq(sequenceNumberExpression, streamPosition.Value))
                    .Project(projection)
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
        #nullable disable

        /// <summary>
        /// Fetches a range of events that has a sequence number inclusive between the <see cref="StreamPositionRange" />.
        /// </summary>
        /// <typeparam name="TEvent">The type of the stored event.</typeparam>
        /// <param name="stream">The <see cref="IMongoCollection{TDocument}" />.</param>
        /// <param name="filter">The <see cref="FilterDefinitionBuilder{TDocument}" />.</param>
        /// <param name="sequenceNumberExpression">The <see cref="Expression{T}" /> for getting the sequence number from the stored event.</param>
        /// <param name="projection">The <see cref="ProjectionDefinition{TSource, TProjection}" /> for projecting the stored event to a <see cref="Runtime.Events.Store.Streams.StreamEvent" />.</param>
        /// <param name="range">The <see cref="StreamPositionRange" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A list of events.</returns>
        public static async Task<IEnumerable<Runtime.Events.Store.Streams.StreamEvent>> FetchRange<TEvent>(
            IMongoCollection<TEvent> stream,
            FilterDefinitionBuilder<TEvent> filter,
            Expression<Func<TEvent, ulong>> sequenceNumberExpression,
            ProjectionDefinition<TEvent, Runtime.Events.Store.Streams.StreamEvent> projection,
            StreamPositionRange range,
            CancellationToken cancellationToken)
            where TEvent : class
        {
            try
            {
                var maxNumEvents = range.Length;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                var events = await stream.Find(
                        filter.Gte(sequenceNumberExpression, range.From.Value)
                            & filter.Lte(sequenceNumberExpression, range.From.Value + range.Length))
                    .Limit(limit)
                    .Project(projection)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                return events.ToArray();
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <summary>
        /// Fins the sequence number of the next event that matches the partition filter and has a sequence number greater than or equal to the given position.
        /// </summary>
        /// <typeparam name="TEvent">The type of the stored event.</typeparam>
        /// <param name="stream">The <see cref="IMongoCollection{TDocument}" />.</param>
        /// <param name="filter">The <see cref="FilterDefinitionBuilder{TDocument}" />.</param>
        /// <param name="eventsInPartitionFilter">The <see cref="FilterDefinition{TDocument}" /> for filtering events in the wanted partition.</param>
        /// <param name="sequenceNumberExpression">The <see cref="Expression{T}" /> for getting the sequence number from the stored event.</param>
        /// <param name="fromPosition">The from <see cref="StreamPosition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The position of the next event.</returns>
        public static async Task<StreamPosition> FindNext<TEvent>(
            IMongoCollection<TEvent> stream,
            FilterDefinitionBuilder<TEvent> filter,
            FilterDefinition<TEvent> eventsInPartitionFilter,
            Expression<Func<TEvent, ulong>> sequenceNumberExpression,
            StreamPosition fromPosition,
            CancellationToken cancellationToken)
            where TEvent : class
        {
            try
            {
                var streamEvent = await stream.Find(
                    eventsInPartitionFilter
                    & filter.Gte(sequenceNumberExpression, fromPosition.Value))
                    .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                return streamEvent != default ? sequenceNumberExpression.Compile()(streamEvent) : ulong.MaxValue;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Runtime.Events.Store.Streams.StreamEvent> Fetch(ScopeId scope, StreamId stream, StreamPosition streamPosition, CancellationToken cancellationToken)
        {
            if (TryGetFetcher(stream, out var fetcher)) return await fetcher.Fetch(scope, stream, streamPosition, cancellationToken).ConfigureAwait(false);
            var streamEvents = await _connection.GetStreamCollection(scope, stream, cancellationToken).ConfigureAwait(false);
            var committedEventWithPartition = await Fetch(
                streamEvents,
                _streamEventFilter,
                _ => _.StreamPosition,
                Builders<Events.StreamEvent>.Projection.Expression(_ => _.ToRuntimeStreamEvent(stream)),
                streamPosition,
                cancellationToken).ConfigureAwait(false);
            if (committedEventWithPartition == default) throw new NoEventInStreamAtPosition(scope, stream, streamPosition);
            return committedEventWithPartition;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Runtime.Events.Store.Streams.StreamEvent>> FetchRange(ScopeId scope, StreamId stream, StreamPositionRange range, CancellationToken cancellationToken)
        {
            if (TryGetFetcher(stream, out var fetcher)) return await fetcher.FetchRange(scope, stream, range, cancellationToken).ConfigureAwait(false);
            var streamEvents = await _connection.GetStreamCollection(scope, stream, cancellationToken).ConfigureAwait(false);
            return await FetchRange(
                streamEvents,
                _streamEventFilter,
                _ => _.StreamPosition,
                Builders<Events.StreamEvent>.Projection.Expression(_ => _.ToRuntimeStreamEvent(stream)),
                range,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<StreamPosition> FindNext(ScopeId scope, StreamId stream, PartitionId partition, StreamPosition fromPosition, CancellationToken cancellationToken)
        {
            if (TryGetFetcher(stream, out var fetcher)) return await fetcher.FindNext(scope, stream, partition, fromPosition, cancellationToken).ConfigureAwait(false);
            var streamEvents = await _connection.GetStreamCollection(scope, stream, cancellationToken).ConfigureAwait(false);
            return await FindNext(
                streamEvents,
                _streamEventFilter,
                _streamEventFilter.Eq(_ => _.Partition, partition.Value),
                _ => _.StreamPosition,
                fromPosition,
                cancellationToken).ConfigureAwait(false);
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
    }
}
