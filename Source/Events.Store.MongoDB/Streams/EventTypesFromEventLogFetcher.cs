// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractEventTypesFromWellKnownStreamsFetcher" /> that fetches event types from the event log.
    /// </summary>
    public class EventTypesFromEventLogFetcher : AbstractEventTypesFromWellKnownStreamsFetcher
    {
        readonly FilterDefinitionBuilder<Event> _eventLogFilter = Builders<Event>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypesFromEventLogFetcher"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventTypesFromEventLogFetcher(EventStoreConnection connection, ILogger logger)
            : base(new StreamId[] { StreamId.AllStreamId })
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<IEnumerable<Artifact>> FetchInRange(ScopeId scope, StreamId stream, StreamPositionRange range, CancellationToken cancellationToken) =>
            FetchInRangeAndPartition(scope, stream, PartitionId.NotSet, range, cancellationToken);

        /// <inheritdoc/>
        public override async Task<IEnumerable<Artifact>> FetchInRangeAndPartition(ScopeId scope, StreamId stream, PartitionId partition, StreamPositionRange range, CancellationToken cancellationToken)
        {
            if (!CanFetchFromStream(stream)) throw new EventTypesFromWellKnownStreamsFetcherCannotFetchFromStream(this, stream);
            if (partition != PartitionId.NotSet) return Enumerable.Empty<Artifact>();
            return await EventTypesFromStreamsFetcher.FetchInRange(
                await _connection.GetEventLogCollection(scope, cancellationToken).ConfigureAwait(false),
                _eventLogFilter,
                _eventLogFilter.Empty,
                _ => _.EventLogSequenceNumber,
                Builders<Event>.Projection.Expression(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration)),
                range,
                cancellationToken).ConfigureAwait(false);
        }
    }
}