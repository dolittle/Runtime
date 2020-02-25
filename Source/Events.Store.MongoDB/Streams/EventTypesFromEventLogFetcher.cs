// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
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
        public override async Task<IEnumerable<Artifact>> FetchTypesInRange(StreamId stream, StreamPosition fromPosition, StreamPosition toPosition, CancellationToken cancellationToken = default)
        {
            if (!CanFetchFromStream(stream)) throw new EventTypesFromWellKnownStreamsFetcherCannotFetchFromStream(this, stream);
            try
            {
                var eventTypes = await _connection.EventLog
                    .Find(_eventLogFilter.Gte(_ => _.EventLogVersion, fromPosition.Value) & _eventLogFilter.Lte(_ => _.EventLogVersion, toPosition.Value))
                    .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return new HashSet<Artifact>(eventTypes);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<Artifact>> FetchTypesInRangeAndPartition(StreamId stream, PartitionId partition, StreamPosition fromPosition, StreamPosition toPosition, CancellationToken cancellationToken = default)
        {
            if (!CanFetchFromStream(stream)) throw new EventTypesFromWellKnownStreamsFetcherCannotFetchFromStream(this, stream);
            if (partition != PartitionId.NotSet) return Enumerable.Empty<Artifact>();
            try
            {
                var eventTypes = await _connection.EventLog
                    .Find(_eventLogFilter.Gte(_ => _.EventLogVersion, fromPosition.Value) & _eventLogFilter.Lte(_ => _.EventLogVersion, toPosition.Value))
                    .Project(_ => new Artifact(_.Metadata.TypeId, _.Metadata.TypeGeneration))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                return new HashSet<Artifact>(eventTypes);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}