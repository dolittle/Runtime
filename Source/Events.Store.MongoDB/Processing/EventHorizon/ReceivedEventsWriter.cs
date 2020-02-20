// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.EventHorizon;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IWriteReceivedEvents" />.
    /// </summary>
    public class ReceivedEventsWriter : IWriteReceivedEvents
    {
        readonly FilterDefinitionBuilder<ReceivedEvent> _streamEventFilter = Builders<ReceivedEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedEventsWriter"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public ReceivedEventsWriter(
            EventStoreConnection connection,
            ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Write(CommittedEvent @event, Microservice producerMicroservice, TenantId producerTenant, CancellationToken cancellationToken)
        {
            StreamPosition streamPosition = null;
            try
            {
                using var session = await _connection.MongoClient.StartSessionAsync().ConfigureAwait(false);
                await session.WithTransactionAsync(
                    async (transaction, cancellationToken) =>
                    {
                        var receivedEvents = await _connection.GetReceivedEventsCollectionAsync(producerMicroservice, cancellationToken).ConfigureAwait(false);
                        streamPosition = (uint)await receivedEvents.CountDocumentsAsync(
                            transaction,
                            _streamEventFilter.Empty).ConfigureAwait(false);

                        await receivedEvents.InsertOneAsync(
                            transaction,
                            @event.ToNewReceivedEvent(streamPosition, producerTenant),
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
                throw new EventAlreadyWrittenToStream(@event.Type.Id, @event.EventLogVersion, producerMicroservice.Value);
            }
            catch (MongoWriteException exception)
            {
                if (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    throw new EventAlreadyWrittenToStream(@event.Type.Id, @event.EventLogVersion, producerMicroservice.Value);
                }

                throw;
            }
            catch (MongoBulkWriteException exception)
            {
                foreach (var error in exception.WriteErrors)
                {
                    if (error.Category == ServerErrorCategory.DuplicateKey)
                    {
                        throw new EventAlreadyWrittenToStream(@event.Type.Id, @event.EventLogVersion, producerMicroservice.Value);
                    }
                }

                throw;
            }
        }
    }
}