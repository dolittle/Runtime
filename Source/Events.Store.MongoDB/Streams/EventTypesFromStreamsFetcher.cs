// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Types;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventTypesFromStreams" />.
    /// </summary>
    public class EventTypesFromStreamsFetcher : IFetchEventTypesFromStreams
    {
        readonly IEnumerable<ICanFetchEventTypesFromWellKnownStreams> _wellKnownStreamFetchers;
        readonly FilterDefinitionBuilder<Events.StreamEvent> _streamEventFilter = Builders<Events.StreamEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypesFromStreamsFetcher"/> class.
        /// </summary>
        /// <param name="wellKnownStreamFetchers">The instances of <see cref="ICanFetchEventTypesFromWellKnownStreams" />.</param>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventTypesFromStreamsFetcher(
            IInstancesOf<ICanFetchEventTypesFromWellKnownStreams> wellKnownStreamFetchers,
            EventStoreConnection connection,
            ILogger logger)
        {
            _wellKnownStreamFetchers = wellKnownStreamFetchers;
            _connection = connection;
            _logger = logger;
        }

        /// <summary>
        /// Fetches a list of <see cref="Artifact" /> in a range.
        /// </summary>
        /// <typeparam name="TEvent">The type of the stored event.</typeparam>
        /// <param name="stream">The <see cref="IMongoCollection{TDocument}" />.</param>
        /// <param name="filter">The <see cref="FilterDefinitionBuilder{TDocument}" />.</param>
        /// <param name="eventsInPartitionFilter">The <see cref="FilterDefinition{TDocument}" /> for filtering events in the wanted partition.</param>
        /// <param name="sequenceNumberExpression">The <see cref="Expression{T}" /> for getting the sequence number from the stored event.</param>
        /// <param name="projection">The <see cref="ProjectionDefinition{TSource, TProjection}" /> for projecting the stored event to a <see cref="StreamEvent" />.</param>
        /// <param name="range">The from <see cref="StreamPosition" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The position of the next event.</returns>
        public static async Task<IEnumerable<Artifact>> FetchInRange<TEvent>(
            IMongoCollection<TEvent> stream,
            FilterDefinitionBuilder<TEvent> filter,
            FilterDefinition<TEvent> eventsInPartitionFilter,
            Expression<Func<TEvent, ulong>> sequenceNumberExpression,
            ProjectionDefinition<TEvent, Artifact> projection,
            StreamPositionRange range,
            CancellationToken cancellationToken)
            where TEvent : class
        {
            try
            {
                var maxNumEvents = range.Length;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                return await stream
                    .Find(eventsInPartitionFilter & filter.Gte(sequenceNumberExpression, range.From.Value)
                        & filter.Lte(sequenceNumberExpression, range.From.Value + range.Length))
                    .Limit(limit)
                    .Project(projection)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Artifact>> FetchInRange(
            ScopeId scope,
            StreamId stream,
            StreamPositionRange range,
            CancellationToken cancellationToken)
        {
            if (TryGetFetcher(stream, out var fetcher)) await fetcher.FetchInRange(scope, stream, range, cancellationToken).ConfigureAwait(false);
            return await FetchInRange(
                await _connection.GetStreamCollection(scope, stream, cancellationToken).ConfigureAwait(false),
                _streamEventFilter,
                _streamEventFilter.Empty,
                _ => _.StreamPosition,
                Builders<Events.StreamEvent>.Projection.Expression(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration)),
                range,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Artifact>> FetchInRangeAndPartition(
            ScopeId scope,
            StreamId stream,
            PartitionId partition,
            StreamPositionRange range,
            CancellationToken cancellationToken)
        {
            if (TryGetFetcher(stream, out var fetcher)) await fetcher.FetchInRangeAndPartition(scope, stream, partition, range, cancellationToken).ConfigureAwait(false);
            return await FetchInRange(
                await _connection.GetStreamCollection(scope, stream, cancellationToken).ConfigureAwait(false),
                _streamEventFilter,
                _streamEventFilter.Eq(_ => _.Partition, partition.Value),
                _ => _.StreamPosition,
                Builders<Events.StreamEvent>.Projection.Expression(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration)),
                range,
                cancellationToken).ConfigureAwait(false);
        }

        bool TryGetFetcher(StreamId stream, out ICanFetchEventTypesFromWellKnownStreams fetcher)
        {
            fetcher = null;
            foreach (var instance in _wellKnownStreamFetchers)
            {
                if (instance.CanFetchFromStream(stream))
                {
                    if (fetcher != null) throw new MultipleWellKnownStreamEventTypesFetchers(stream);
                    fetcher = instance;
                }
            }

            return fetcher != null;
        }
    }
}
