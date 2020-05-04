// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.EventHorizon.Producer;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanFetchEventsFromPublicStreams" />.
    /// </summary>
    public class EventsFromPublicStreamsFetcher : ICanFetchEventsFromPublicStreams
    {
        readonly FilterDefinitionBuilder<Events.StreamEvent> _filter = Builders<Events.StreamEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromPublicStreamsFetcher"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="EventStoreConnection" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventsFromPublicStreamsFetcher(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Runtime.Events.Store.Streams.StreamEvent> Fetch(StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken)
        {
            return await EventsFromStreamsFetcher.Fetch(
                await _connection.GetPublicStreamCollection(streamId, cancellationToken).ConfigureAwait(false),
                _filter,
                _ => _.StreamPosition,
                Builders<Events.StreamEvent>.Projection.Expression(_ => _.ToRuntimeStreamEvent(streamId)),
                streamPosition,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<Runtime.Events.Store.Streams.StreamEvent> Fetch(ScopeId scope, StreamId streamId, StreamPosition streamPosition, CancellationToken cancellationToken) =>
            Fetch(streamId, streamPosition, cancellationToken);

        /// <inheritdoc/>
        public async Task<IEnumerable<Runtime.Events.Store.Streams.StreamEvent>> FetchRange(StreamId streamId, StreamPositionRange range, CancellationToken cancellationToken)
        {
            return await EventsFromStreamsFetcher.FetchRange(
                await _connection.GetPublicStreamCollection(streamId, cancellationToken).ConfigureAwait(false),
                _filter,
                _ => _.StreamPosition,
                Builders<Events.StreamEvent>.Projection.Expression(_ => _.ToRuntimeStreamEvent(streamId)),
                range,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Runtime.Events.Store.Streams.StreamEvent>> FetchRange(ScopeId scope, StreamId streamId, StreamPositionRange range, CancellationToken cancellationToken) =>
            FetchRange(streamId, range, cancellationToken);

        /// <inheritdoc/>
        public async Task<StreamPosition> FindNext(StreamId streamId, PartitionId partition, StreamPosition fromPosition, CancellationToken cancellationToken)
        {
            if (partition != PartitionId.NotSet) return ulong.MaxValue;
            return await EventsFromStreamsFetcher.FindNext(
                await _connection.GetPublicStreamCollection(streamId, cancellationToken).ConfigureAwait(false),
                _filter,
                _filter.Empty,
                _ => _.StreamPosition,
                fromPosition,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(ScopeId scope, StreamId streamId, PartitionId partitionId, StreamPosition fromPosition, CancellationToken cancellationToken) =>
            FindNext(streamId, partitionId, fromPosition, cancellationToken);
    }
}
