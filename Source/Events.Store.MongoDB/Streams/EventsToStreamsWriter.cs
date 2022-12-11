// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IWriteEventsToStreams" /> and <see cref="IWriteEventsToStreamCollection" />..
    /// </summary>
    public class EventsToStreamsWriter : IWriteEventsToStreamCollection, IWriteEventsToStreams
    {
        readonly FilterDefinitionBuilder<Events.StreamEvent> _streamFilter = Builders<Events.StreamEvent>.Filter;
        readonly IStreams _streams;
        readonly IEventConverter _eventConverter;
        readonly IStreamEventWatcher _streamWatcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsToStreamsWriter"/> class.
        /// </summary>
        /// <param name="streams">The <see cref="IStreams"/>.</param>
        /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
        /// <param name="streamWatcher">The <see cref="IStreamEventWatcher" />.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public EventsToStreamsWriter(IStreams streams, IEventConverter eventConverter, IStreamEventWatcher streamWatcher, ILogger logger)
        {
            _streams = streams;
            _eventConverter = eventConverter;
            _streamWatcher = streamWatcher;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Write(CommittedEvent @event, ScopeId scope, StreamId stream, PartitionId partition, CancellationToken cancellationToken)
        {
            _logger.WritingEventToStream(@event.EventLogSequenceNumber, stream, scope);
            var writtenStreamPosition = await Write(
                await _streams.Get(scope, stream, cancellationToken).ConfigureAwait(false),
                _streamFilter,
                streamPosition =>
                    @event is CommittedExternalEvent externalEvent ?
                        _eventConverter.ToStoreStreamEvent(externalEvent, streamPosition, partition)
                        : _eventConverter.ToStoreStreamEvent(@event, streamPosition, partition),
                cancellationToken).ConfigureAwait(false);
            _streamWatcher.NotifyForEvent(scope, stream, writtenStreamPosition);
        }

        /// <inheritdoc/>
        public async Task<StreamPosition> Write<TEvent>(
            IMongoCollection<TEvent> stream,
            FilterDefinitionBuilder<TEvent> filter,
            Func<StreamPosition, TEvent> createStoreEvent,
            CancellationToken cancellationToken)
            where TEvent : class
        {
            try
            {
                using var session = await _streams.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                try
                {
                    session.StartTransaction();
                    var streamPosition = (ulong)await stream.CountDocumentsAsync(
                        session,
                        filter.Empty,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    var eventToWrite = createStoreEvent(streamPosition);
                    await stream.InsertOneAsync(session, eventToWrite, cancellationToken: cancellationToken).ConfigureAwait(false);
                    await session.CommitTransactionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                    return streamPosition;
                }
                catch
                {
                    await session.AbortTransactionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
            catch (MongoWaitQueueFullException ex)
            {
                throw new EventStoreUnavailable("Mongo wait queue is full", ex);
            }
            catch (Exception ex) when (typeof(TEvent) == typeof(Events.StreamEvent) && IsDuplicateKeyException(ex))
            {
                using var session = await _streams.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                try
                {
                    session.StartTransaction();
                    var streamPosition = (ulong) await stream.CountDocumentsAsync(
                        session,
                        Builders<TEvent>.Filter.Empty,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (streamPosition == 0)
                    {
                        throw;
                    }
                    return streamPosition - 1;
                }
                catch
                {
                    await session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
        }

        static bool IsDuplicateKeyException(Exception exception)
            => exception switch
            {
                MongoDuplicateKeyException => true,
                MongoWriteException writeException => writeException.Message.Contains("duplicate key error"),
                MongoBulkWriteException bulkWriteException => bulkWriteException.Message.Contains("duplicate key error"),
                _ => false,
            };
    }
}
