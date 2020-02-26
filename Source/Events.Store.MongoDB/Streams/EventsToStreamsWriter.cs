// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Types;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IWriteEventsToStreams" />.
    /// </summary>
    public class EventsToStreamsWriter : IWriteEventsToStreams
    {
        readonly IEnumerable<ICanWriteEventsToWellKnownStreams> _wellKnownStreamWriters;
        readonly FilterDefinitionBuilder<MongoDB.Events.StreamEvent> _streamEventFilter = Builders<MongoDB.Events.StreamEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsToStreamsWriter"/> class.
        /// </summary>
        /// <param name="wellKnownStreamWriters">The instances of <see cref="ICanWriteEventsToWellKnownStreams" />.</param>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventsToStreamsWriter(
            IInstancesOf<ICanWriteEventsToWellKnownStreams> wellKnownStreamWriters,
            EventStoreConnection connection,
            ILogger logger)
        {
            _wellKnownStreamWriters = wellKnownStreamWriters;
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task Write(CommittedEvent @event, StreamId stream, PartitionId partition, CancellationToken cancellationToken = default)
        {
            if (TryGetWriter(stream, out var writer)) return writer.Write(@event, stream, partition, cancellationToken);
            return WriteToStream(@event, stream, partition, cancellationToken);
        }

        bool TryGetWriter(StreamId stream, out ICanWriteEventsToWellKnownStreams writer)
        {
            writer = null;
            foreach (var instance in _wellKnownStreamWriters)
            {
                if (instance.CanWriteToStream(stream))
                {
                    if (writer != null) throw new MultipleWellKnownStreamWriters(stream);
                    writer = instance;
                }
            }

            return writer != null;
        }

        async Task WriteToStream(CommittedEvent @event, StreamId streamId, PartitionId partitionId, CancellationToken cancellationToken)
        {
            ThrowIfWritingToAllStream(streamId);
            StreamPosition streamPosition = null;
            try
            {
                using var session = await _connection.MongoClient.StartSessionAsync().ConfigureAwait(false);
                await session.WithTransactionAsync(
                    async (transaction, cancellationToken) =>
                    {
                        var stream = await _connection.GetStreamCollectionAsync(streamId, cancellationToken).ConfigureAwait(false);
                        streamPosition = (uint)await stream.CountDocumentsAsync(
                            transaction,
                            _streamEventFilter.Empty).ConfigureAwait(false);

                        await stream.InsertOneAsync(
                            transaction,
                            @event.ToStoreStreamEvent(streamPosition, partitionId),
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
                throw new EventAlreadyWrittenToStream(@event.Type.Id, @event.EventLogSequenceNumber, streamId);
            }
            catch (MongoWriteException exception)
            {
                if (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    throw new EventAlreadyWrittenToStream(@event.Type.Id, @event.EventLogSequenceNumber, streamId);
                }

                throw;
            }
            catch (MongoBulkWriteException exception)
            {
                foreach (var error in exception.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        throw new EventAlreadyWrittenToStream(@event.Type.Id, @event.EventLogSequenceNumber, streamId);
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