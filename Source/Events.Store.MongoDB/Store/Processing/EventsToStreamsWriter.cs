// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IWriteEventsToStreams" />.
    /// </summary>
    public class EventsToStreamsWriter : IWriteEventsToStreams
    {
        readonly FilterDefinitionBuilder<Event> _streamEventFilter = Builders<Event>.Filter;
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
            if (@event == null) throw new EventCanNotBeNull();
            ThrowIfWritingToAllStream(streamId);
            StreamPosition streamPosition = null;
            try
            {
                using var session = await _connection.MongoClient.StartSessionAsync().ConfigureAwait(false);
                await session.WithTransactionAsync(
                    async (transaction, cancellationToken) =>
                    {
                        var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                        streamPosition = (StreamPosition)await stream.CountDocumentsAsync(
                            transaction,
                            _streamEventFilter.Empty).ConfigureAwait(false);

                        await stream.InsertOneAsync(
                            transaction,
                            @event.ToStoreRepresentation(streamPosition, partitionId),
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                        return Task.CompletedTask;
                    },
                    null,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
             catch (MongoDuplicateKeyException)
            {
                throw new StreamPositionOccupied(streamPosition, streamId);
            }
            catch (MongoWriteException exception)
            {
                if (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    throw new StreamPositionOccupied(streamPosition, streamId);
                }

                throw;
            }
            catch (MongoBulkWriteException exception)
            {
                foreach (var error in exception.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        throw new StreamPositionOccupied(streamPosition, streamId);
                    }
                }

                throw;
            }
        }

        void ThrowIfWritingToAllStream(StreamId streamId)
        {
            if (streamId.Value == StreamId.AllStreamId.Value) throw new CannotWriteCommittedEventToAllStream();
        }
    }
}