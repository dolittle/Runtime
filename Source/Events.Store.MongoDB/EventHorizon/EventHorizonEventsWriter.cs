// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IWriteEventHorizonEvents" />.
    /// </summary>
    public class EventHorizonEventsWriter : IWriteEventHorizonEvents
    {
        readonly FilterDefinitionBuilder<EventHorizonEvent> _streamEventFilter = Builders<EventHorizonEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonEventsWriter"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventHorizonEventsWriter(
            EventStoreConnection connection,
            ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Write(CommittedEvent @event, Runtime.EventHorizon.EventHorizon eventHorizon, CancellationToken cancellationToken = default)
        {
            StreamPosition streamPosition = null;
            try
            {
                using var session = await _connection.MongoClient.StartSessionAsync().ConfigureAwait(false);
                await session.WithTransactionAsync(
                    async (transaction, cancellationToken) =>
                    {
                        var eventHorizonEvents = await _connection.GetEventHorizonEventsCollectionAsync(eventHorizon.ProducerMicroservice, cancellationToken).ConfigureAwait(false);
                        streamPosition = (uint)await eventHorizonEvents.CountDocumentsAsync(
                            transaction,
                            _streamEventFilter.Empty).ConfigureAwait(false);

                        await eventHorizonEvents.InsertOneAsync(
                            transaction,
                            @event.ToNewEventHorizonEvent(streamPosition, eventHorizon),
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
                throw new EventAlreadyWrittenToStream(@event.Type.Id, @event.EventLogSequenceNumber, eventHorizon.ProducerMicroservice.Value);
            }
            catch (MongoWriteException exception)
            {
                if (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    throw new EventAlreadyWrittenToStream(@event.Type.Id, @event.EventLogSequenceNumber, eventHorizon.ProducerMicroservice.Value);
                }

                throw;
            }
            catch (MongoBulkWriteException exception)
            {
                foreach (var error in exception.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        throw new EventAlreadyWrittenToStream(@event.Type.Id, @event.EventLogSequenceNumber, eventHorizon.ProducerMicroservice.Value);
                    }
                }

                throw;
            }
        }
    }
}