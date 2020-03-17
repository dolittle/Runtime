// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
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
        readonly FilterDefinitionBuilder<MongoDB.Events.StreamEvent> _streamEventFilter = Builders<MongoDB.Events.StreamEvent>.Filter;
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

        /// <inheritdoc/>
        public Task<IEnumerable<Artifact>> FetchTypesInRange(StreamId stream, StreamPositionRange range, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidRange(range);
            if (TryGetFetcher(stream, out var fetcher)) fetcher.FetchTypesInRange(stream, range, cancellationToken);
            return FetchTypesInRangeFromStream(stream, range, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Artifact>> FetchTypesInRangeAndPartition(StreamId stream, PartitionId partition, StreamPositionRange range, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidRange(range);
            if (TryGetFetcher(stream, out var fetcher)) fetcher.FetchTypesInRangeAndPartition(stream, partition, range, cancellationToken);
            return FetchTypesInRangeAndPartitionFromStream(stream, partition, range, cancellationToken);
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

        async Task<IEnumerable<Artifact>> FetchTypesInRangeFromStream(StreamId streamId, StreamPositionRange range, CancellationToken cancellationToken)
        {
            try
            {
                var maxNumEvents = range.To.Value - range.From.Value + 1U;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var eventTypes = await stream
                    .Find(_streamEventFilter.Gte(_ => _.StreamPosition, range.From.Value) & _streamEventFilter.Lte(_ => _.StreamPosition, range.To.Value))
                    .Limit(limit)
                    .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return eventTypes;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        async Task<IEnumerable<Artifact>> FetchTypesInRangeAndPartitionFromStream(StreamId streamId, PartitionId partition, StreamPositionRange range, CancellationToken cancellationToken)
        {
            try
            {
                var maxNumEvents = range.To.Value - range.From.Value + 1U;
                int? limit = (int)maxNumEvents;
                if (limit < 0) limit = null;
                var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var eventTypes = await stream
                    .Find(_streamEventFilter.Eq(_ => _.Metadata.Partition, partition.Value) & _streamEventFilter.Gte(_ => _.StreamPosition, range.From.Value) & _streamEventFilter.Lte(_ => _.StreamPosition, range.To.Value))
                    .Limit(limit)
                    .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return eventTypes;
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        void ThrowIfInvalidRange(StreamPositionRange range)
        {
            if (range.From.Value > range.To.Value) throw new FromPositionIsGreaterThanToPosition(range);
        }
    }
}