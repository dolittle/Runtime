// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Filters.EventHorizon;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IWriteEventsToPublicStreams" />.
    /// </summary>
    public class EventsToPublicStreamsWriter : IWriteEventsToPublicStreams
    {
        readonly FilterDefinitionBuilder<Events.StreamEvent> _filter = Builders<Events.StreamEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsToPublicStreamsWriter"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="EventStoreConnection" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventsToPublicStreamsWriter(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task Write(CommittedEvent @event, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken) =>
            Write(@event, ScopeId.Default, streamId, partitionId, cancellationToken);

        /// <inheritdoc/>
        public async Task Write(CommittedEvent @event, ScopeId scope, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken)
        {
            EventsToStreamsWriter.ThrowIfWritingToAllStream(streamId);
            await EventsToStreamsWriter.Write(
                _connection,
                await _connection.GetPublicStreamCollection(streamId, cancellationToken).ConfigureAwait(false),
                _filter,
                streamPosition => @event.ToStoreStreamEvent(streamPosition, partitionId),
                scope,
                streamId,
                @event.Type.Id,
                cancellationToken).ConfigureAwait(false);
        }
    }
}