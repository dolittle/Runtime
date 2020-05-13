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
    public class EventsToStreamsWriter : IEventsToStreamsWriter, IWriteEventsToStreams
    {
        readonly FilterDefinitionBuilder<Events.StreamEvent> _streamFilter = Builders<Events.StreamEvent>.Filter;
        readonly IStreams _streams;
        readonly IEventConverter _eventConverter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsToStreamsWriter"/> class.
        /// </summary>
        /// <param name="streams">The <see cref="IStreams"/>.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventsToStreamsWriter(IStreams streams, IEventConverter eventConverter, ILogger<EventsToStreamsWriter> logger)
        {
            _streams = streams;
            _eventConverter = eventConverter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Write(CommittedEvent @event, ScopeId scope, StreamId stream, PartitionId partition, CancellationToken cancellationToken)
        {
            _logger.Trace("Writing Event: {EventLogSequenceNumber} to Stream: {Stream} in Scope: {Scope}", @event.EventLogSequenceNumber, stream, scope);
            await Write(
                await _streams.Get(scope, stream, cancellationToken).ConfigureAwait(false),
                _streamFilter,
                streamPosition => _eventConverter.ToStoreStreamEvent(@event, streamPosition, partition),
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task Write<TEvent>(
            IMongoCollection<TEvent> stream,
            FilterDefinitionBuilder<TEvent> filter,
            Func<StreamPosition, TEvent> createStoreEvent,
            CancellationToken cancellationToken)
            where TEvent : class
        {
            StreamPosition streamPosition = null;
            try
            {
                using var session = await _streams.StartSessionAsync().ConfigureAwait(false);
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
    }
}
