// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

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
            return streamPosition;
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }
}