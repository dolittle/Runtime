// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IWriteEventsToStreams" />.
    /// </summary>
    public class EventsToStreamsWriter : IWriteEventsToStreams
    {
        readonly FilterDefinitionBuilder<StreamEvent> _streamEventFilter = Builders<StreamEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsToStreamsWriter"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventsToStreamsWriter(EventStoreConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Write(CommittedEvent @event, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken = default)
        {
            try
            {
                using var session = await _connection.MongoClient.StartSessionAsync().ConfigureAwait(false);

                await session.WithTransactionAsync(
                    async (transaction, cancel) =>
                    {
                        var events = _connection.StreamEvents;
                        var numDocuments = await events.CountDocumentsAsync(
                            transaction,
                            _streamEventFilter.Eq(_ => _.StreamIdAndPosition.StreamId, streamId.Value)).ConfigureAwait(false);

                        var streamPosition = new StreamPosition((uint)numDocuments);
                        await events.InsertOneAsync(
                            transaction,
                            @event.ToStreamEvent(streamId, streamPosition, partitionId),
                            null,
                            cancel).ConfigureAwait(false);
                        return Task.CompletedTask;
                    },
                    null,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }
    }
}