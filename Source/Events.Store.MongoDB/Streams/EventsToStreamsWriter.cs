// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Represents an implementation of <see cref="IWriteEventsToStreams" /> and <see cref="IWriteEventsToStreamCollection" />..
/// </summary>
[DisableAutoRegistration]
public class EventsToStreamsWriter : IWriteEventsToStreamCollection, IWriteEventsToStreams
{
    readonly FilterDefinitionBuilder<Events.StreamEvent> _streamFilter = Builders<Events.StreamEvent>.Filter;
    readonly IStreams _streams;
    readonly IEventConverter _eventConverter;
    readonly IStreamEventWatcher _streamWatcher;
    readonly IGetUniqueEventsToWrite _uniqueEventsToWriteGetter;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventsToStreamsWriter"/> class.
    /// </summary>
    /// <param name="streams">The <see cref="IStreams"/>.</param>
    /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
    /// <param name="streamWatcher">The <see cref="IStreamEventWatcher" />.</param>
    /// <param name="uniqueEventsToWriteGetter">The <see cref="IGetUniqueEventsToWrite"/>.</param>
    /// <param name="logger">An <see cref="ILogger"/>.</param>
    public EventsToStreamsWriter(IStreams streams, IEventConverter eventConverter, IStreamEventWatcher streamWatcher, IGetUniqueEventsToWrite uniqueEventsToWriteGetter, ILogger logger)
    {
        _streams = streams;
        _eventConverter = eventConverter;
        _streamWatcher = streamWatcher;
        _uniqueEventsToWriteGetter = uniqueEventsToWriteGetter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task Write(CommittedEvent @event, ScopeId scope, StreamId stream, PartitionId partition, CancellationToken cancellationToken)
    {
        _logger.WritingEventToStream(@event.EventLogSequenceNumber, stream, scope);
        var collection = await _streams.Get(scope, stream, cancellationToken).ConfigureAwait(false);
        try
        {
            var writtenStreamPosition = await Write(
                collection,
                _streamFilter,
                streamPosition =>
                    @event is CommittedExternalEvent externalEvent ?
                        _eventConverter.ToStoreStreamEvent(externalEvent, streamPosition, partition)
                        : _eventConverter.ToStoreStreamEvent(@event, streamPosition, partition),
                cancellationToken).ConfigureAwait(false);
            _streamWatcher.NotifyForEvent(scope, stream, writtenStreamPosition);

        }
        catch (MongoDuplicateKeyException)
        {
            var lastStoredEvent = await collection
                .Find(_streamFilter.Empty)
                .Sort(Builders<Events.StreamEvent>.Sort.Descending(_ => _.StreamPosition))
                .Limit(1)
                .SingleAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task Write(IEnumerable<(CommittedEvent, PartitionId)> events, ScopeId scope, StreamId stream, CancellationToken cancellationToken)
    {
        try
        {
            var writtenStreamPosition = await Write(
                await _streams.Get(scope, stream, cancellationToken).ConfigureAwait(false),
                _streamFilter,
                streamPosition =>
                {
                    return events.Select((eventAndPartition, i) =>
                        eventAndPartition.Item1 is CommittedExternalEvent externalEvent ?
                            _eventConverter.ToStoreStreamEvent(externalEvent, streamPosition + (ulong)i, eventAndPartition.Item2)
                            : _eventConverter.ToStoreStreamEvent(eventAndPartition.Item1, streamPosition + (ulong)i, eventAndPartition.Item2));
                },
                cancellationToken).ConfigureAwait(false);
            _streamWatcher.NotifyForEvent(scope, stream, writtenStreamPosition);
        }
        catch (MongoDuplicateKeyException e)
        {
            
        }
    }

    /// <inheritdoc/>
    public async Task<StreamPosition> Write<TEvent>(
        IMongoCollection<TEvent> stream,
        FilterDefinitionBuilder<TEvent> filter,
        Func<StreamPosition, TEvent> createStoreEvent,
        CancellationToken cancellationToken)
        where TEvent : IStoredEvent
    {
        StreamPosition streamPosition = null;
        try
        {
            using var session = await _streams.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            await session.WithTransactionAsync(
                async (transaction, cancellationToken) =>
                {
                    streamPosition = (ulong) await stream.CountDocumentsAsync(
                        transaction,
                        filter.Empty,
                        cancellationToken: cancellationToken).ConfigureAwait(false);

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

    /// <inheritdoc />
    public async Task<StreamPosition> Write<TEvent>(IMongoCollection<TEvent> stream, FilterDefinitionBuilder<TEvent> filter, Func<StreamPosition, IEnumerable<TEvent>> createStoreEvents, CancellationToken cancellationToken)
        where TEvent : IStoredEvent
    {
        StreamPosition streamPosition = null;
        try
        {
            using var session = await _streams.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            return await session.WithTransactionAsync(
                async (transaction, cancellationToken) =>
                {
                    streamPosition = (ulong) await stream.CountDocumentsAsync(
                        transaction,
                        filter.Empty, cancellationToken: cancellationToken).ConfigureAwait(false);

                    var eventsToWrite = createStoreEvents(streamPosition).ToArray();
                    try
                    {
                        await stream.InsertManyAsync(
                            transaction,
                            eventsToWrite,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                    catch (MongoDuplicateKeyException)
                    {
                        
                    }
                    return streamPosition + (ulong) (eventsToWrite.Length - 1);
                },
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    async Task WriteEvents<TEvent>(IMongoCollection<TEvent> stream, IReadOnlyList<TEvent> eventsToWrite, IClientSessionHandle transaction, CancellationToken cancellationToken)
        where TEvent : IStoredEvent
    {
        Task WriteToCollection(IReadOnlyList<TEvent> events) => events.Count switch
        {
            0 => Task.CompletedTask,
            1 => stream.InsertOneAsync(transaction, events[0], cancellationToken: cancellationToken),
            _ => stream.InsertManyAsync(transaction, events, cancellationToken: cancellationToken)
        };
        
        try
        {
            await WriteToCollection(eventsToWrite).ConfigureAwait(false);
        }
        catch (MongoDuplicateKeyException)
        {
            var storedEvents = await GetStoredEvents(stream, eventsToWrite, transaction, cancellationToken).ConfigureAwait(false);
            if (!_uniqueEventsToWriteGetter.TryGet(eventsToWrite, storedEvents, out eventsToWrite, out var duplicateEventLogSequenceNumber))
            {
                throw new EventWithEventLogSequenceNumberAlreadyExistsInStream(duplicateEventLogSequenceNumber, stream.CollectionNamespace.CollectionName);
            }
            await WriteToCollection(eventsToWrite).ConfigureAwait(false);
        }
    }

    static Task<List<TEvent>> GetStoredEvents<TEvent>(IMongoCollection<TEvent> stream, IReadOnlyList<TEvent> eventsToWrite, IClientSessionHandle transaction, CancellationToken cancellationToken)
        where TEvent : IStoredEvent
        => stream
            .Find(Builders<TEvent>.Filter.Gte(_ => _.GetEventLogSequenceNumber().Value, eventsToWrite[0].GetEventLogSequenceNumber().Value)
                    & Builders<TEvent>.Filter.Lte(_ => _.GetEventLogSequenceNumber().Value, eventsToWrite[^1].GetEventLogSequenceNumber().Value))
            .SortByDescending(_ => _.GetEventLogSequenceNumber().Value)
            .Limit(eventsToWrite.Count)
            .ToListAsync(cancellationToken);
}
