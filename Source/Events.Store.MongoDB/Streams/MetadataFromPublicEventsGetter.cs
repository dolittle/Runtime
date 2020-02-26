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
    /// Represents an implementation of <see cref="AbstractMetadataFromWellKnownStreamsGetter" /> that can get stream metadata from public events.
    /// </summary>
    public class MetadataFromPublicEventsGetter : AbstractMetadataFromWellKnownStreamsGetter
    {
        readonly FilterDefinitionBuilder<PublicEvent> _publicEventsFilter = Builders<PublicEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataFromPublicEventsGetter"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public MetadataFromPublicEventsGetter(
            EventStoreConnection connection,
            ILogger logger)
            : base(new StreamId[] { StreamId.PublicEventsId })
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<EventLogSequenceNumber> GetLastProcessedEventLogSequenceNumber(StreamId stream, CancellationToken cancellationToken)
        {
            try
            {
                var eventLogSequenceNumbers = await _connection.PublicEvents.Find(_publicEventsFilter.Empty)
                    .SortByDescending(_ => _.Metadata.EventLogSequenceNumber)
                    .Project(_ => _.Metadata.EventLogSequenceNumber)
                    .Limit(1)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);
                if (eventLogSequenceNumbers.Count == 0) return null;
                return eventLogSequenceNumbers[0];
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}