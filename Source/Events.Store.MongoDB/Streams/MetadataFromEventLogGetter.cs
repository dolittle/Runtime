// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractMetadataFromWellKnownStreamsGetter" /> that can get stream metadata from the event log.
    /// </summary>
    public class MetadataFromEventLogGetter : AbstractMetadataFromWellKnownStreamsGetter
    {
        readonly FilterDefinitionBuilder<Event> _eventLogFilter = Builders<Event>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataFromEventLogGetter"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public MetadataFromEventLogGetter(
            EventStoreConnection connection,
            ILogger logger)
            : base(new StreamId[] { StreamId.AllStreamId })
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<EventLogVersion> GetLastProcessedEventLogVersion(StreamId stream, CancellationToken cancellationToken)
        {
            try
            {
                var eventLogVersions = await _connection.EventLog.Find(_eventLogFilter.Empty)
                    .SortByDescending(_ => _.EventLogVersion)
                    .Project(_ => _.EventLogVersion)
                    .Limit(1)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                if (eventLogVersions.Count == 0) return null;
                return eventLogVersions[0];
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}