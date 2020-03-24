// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Types;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IFetchEventsFromStreams" />.
    /// </summary>
    public class StreamsMetadata : IStreamsMetadata
    {
        readonly IEnumerable<ICanGetMetadataFromWellKnownStreams> _wellKNownStreamMetadataGetters;
        readonly FilterDefinitionBuilder<MongoDB.Events.StreamEvent> _streamEventFilter = Builders<MongoDB.Events.StreamEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamsMetadata"/> class.
        /// </summary>
        /// <param name="wellKNownStreamMetadataGetters">The instances of <see cref="ICanGetMetadataFromWellKnownStreams" />.</param>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public StreamsMetadata(
            IInstancesOf<ICanGetMetadataFromWellKnownStreams> wellKNownStreamMetadataGetters,
            EventStoreConnection connection,
            ILogger logger)
        {
            _wellKNownStreamMetadataGetters = wellKNownStreamMetadataGetters;
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<EventLogSequenceNumber> GetLastEventLogSequenceNumber(StreamId stream, CancellationToken cancellationToken)
        {
            if (TryGetMetadataGetter(stream, out var getter)) return getter.GetLastEventLogSequenceNumber(stream, cancellationToken);
            return GetLastEventLogSequenceNumberFromStream(stream, cancellationToken);
        }

        bool TryGetMetadataGetter(StreamId stream, out ICanGetMetadataFromWellKnownStreams getter)
        {
            getter = null;
            foreach (var instance in _wellKNownStreamMetadataGetters)
            {
                if (instance.CanGetMetadataFromStream(stream))
                {
                    if (getter != null) throw new MultipleWellKnownStreamEventFetchers(stream);
                    getter = instance;
                }
            }

            return getter != null;
        }

        async Task<EventLogSequenceNumber> GetLastEventLogSequenceNumberFromStream(StreamId streamId, CancellationToken cancellationToken)
        {
            try
            {
                var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                var eventLogSequenceNumbers = await stream.Find(_streamEventFilter.Empty)
                    .SortByDescending(_ => _.Metadata.EventLogSequenceNumber)
                    .Limit(1)
                    .Project(_ => _.Metadata.EventLogSequenceNumber)
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