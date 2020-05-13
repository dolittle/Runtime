// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IWriteEventsToPublicStreams" />.
    /// </summary>
    public class EventsToPublicStreamsWriter : IWriteEventsToPublicStreams
    {
        readonly FilterDefinitionBuilder<Events.StreamEvent> _filter = Builders<Events.StreamEvent>.Filter;
        readonly IStreams _streams;
        readonly IEventsToStreamsWriter _eventsToStreamsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsToPublicStreamsWriter"/> class.
        /// </summary>
        /// <param name="streams">The <see cref="IStreams" />.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="IEventsToStreamsWriter" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventsToPublicStreamsWriter(IStreams streams, IEventsToStreamsWriter eventsToStreamsWriter, ILogger logger)
        {
            _streams = streams;
            _eventsToStreamsWriter = eventsToStreamsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Write(CommittedEvent @event, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken)
        {
            _logger.Trace("Writing Event: {EventLogSequenceNumber} to Public Stream: {Stream}", @event.EventLogSequenceNumber, streamId);
            await _eventsToStreamsWriter.Write(
                await _streams.GetPublic(streamId, cancellationToken).ConfigureAwait(false),
                _filter,
                streamPosition => @event.ToStoreStreamEvent(streamPosition, partitionId),
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task Write(CommittedEvent @event, ScopeId scope, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken) =>
            Write(@event, streamId, partitionId, cancellationToken);
    }
}