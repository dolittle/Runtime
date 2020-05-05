// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IWriteEventsToStreams" />.
    /// </summary>
    public class EventsToStreamsWriter : IWriteEventsToStreams
    {
        readonly FilterDefinitionBuilder<Events.StreamEvent> _streamEventFilter = Builders<Events.StreamEvent>.Filter;
        readonly EventStoreConnection _connection;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsToStreamsWriter"/> class.
        /// </summary>
        /// <param name="connection">An <see cref="EventStoreConnection"/> to a MongoDB EventStore.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventsToStreamsWriter(
            EventStoreConnection connection,
            ILogger logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <summary>
        /// Writes an event to a stream collection.
        /// </summary>
        /// <param name="connection">The <see cref="EventStoreConnection" />.</param>
        /// <param name="stream">The <see cref="IMongoCollection{TDocument}" /> to write to.</param>
        /// <param name="filter">The <see cref="FilterDefinitionBuilder{TDocument}" /> for the event type.</param>
        /// <param name="createStoreEvent">The callback that creates the event to store.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <typeparam name="TEvent">The type of the stored event.</typeparam>
        /// <returns>A task representing the write transaction.</returns>
        public static async Task Write<TEvent>(
            EventStoreConnection connection,
            IMongoCollection<TEvent> stream,
            FilterDefinitionBuilder<TEvent> filter,
            Func<StreamPosition, TEvent> createStoreEvent,
            CancellationToken cancellationToken)
            where TEvent : class
        {
            StreamPosition streamPosition = null;
            try
            {
                using var session = await connection.MongoClient.StartSessionAsync().ConfigureAwait(false);
                await session.WithTransactionAsync(
                    async (transaction, cancellationToken) =>
                    {
                        streamPosition = (ulong)await stream.CountDocumentsAsync(
                            transaction,
                            filter.Empty).ConfigureAwait(false);

                        await stream.InsertOneAsync(
                            transaction,
                            createStoreEvent(streamPosition),
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                        return Task.CompletedTask;
                    },
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
        }

        /// <inheritdoc/>
        public async Task Write(CommittedEvent @event, ScopeId scope, StreamId stream, PartitionId partition, CancellationToken cancellationToken)
        {
            await Write(
                _connection,
                await _connection.GetStreamCollection(scope, stream, cancellationToken).ConfigureAwait(false),
                _streamEventFilter,
                streamPosition => @event.ToStoreStreamEvent(streamPosition, partition),
                cancellationToken).ConfigureAwait(false);
        }
    }
}