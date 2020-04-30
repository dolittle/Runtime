// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractEventsFromWellKnownStreamsFetcher" /> that can fetch events from the event log.
    /// </summary>
    public class EventFromEventLogFetcher : AbstractEventsFromWellKnownStreamsFetcher
    {
        readonly FilterDefinitionBuilder<MongoDB.Events.Event> _eventLogFilter = Builders<MongoDB.Events.Event>.Filter;
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
        public override async Task<Runtime.Events.Store.Streams.StreamEvent> Fetch(ScopeId scope, StreamId stream, StreamPosition streamPosition, CancellationToken cancellationToken)
        {
            if (!CanFetchFromStream(stream)) throw new EventsFromWellKnownStreamsFetcherCannotFetchFromStream(this, stream);
            return await EventsFromStreamsFetcher.Fetch(
                await _connection.GetEventLogCollection(scope, cancellationToken).ConfigureAwait(false),
                _eventLogFilter,
                _ => _.EventLogSequenceNumber,
                Builders<MongoDB.Events.Event>.Projection.Expression(_ => _.ToRuntimeStreamEvent()),
                streamPosition,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<Runtime.Events.Store.Streams.StreamEvent>> FetchRange(ScopeId scope, StreamId stream, StreamPositionRange range, CancellationToken cancellationToken)
        {
            if (!CanFetchFromStream(stream)) throw new EventsFromWellKnownStreamsFetcherCannotFetchFromStream(this, stream);
            return await EventsFromStreamsFetcher.FetchRange(
                await _connection.GetEventLogCollection(scope, cancellationToken).ConfigureAwait(false),
                _eventLogFilter,
                _ => _.EventLogSequenceNumber,
                Builders<MongoDB.Events.Event>.Projection.Expression(_ => _.ToRuntimeStreamEvent()),
                range,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override async Task<StreamPosition> FindNext(ScopeId scope, StreamId stream, PartitionId partition, StreamPosition fromPosition, CancellationToken cancellationToken)
        {
            if (!CanFetchFromStream(stream)) throw new EventsFromWellKnownStreamsFetcherCannotFetchFromStream(this, stream);
            if (partition != PartitionId.NotSet) return ulong.MaxValue;
            return await EventsFromStreamsFetcher.FindNext(
                await _connection.GetEventLogCollection(scope, cancellationToken).ConfigureAwait(false),
                _eventLogFilter,
                _eventLogFilter.Empty,
                _ => _.EventLogSequenceNumber,
                fromPosition,
                cancellationToken).ConfigureAwait(false);
        }
    }
}
