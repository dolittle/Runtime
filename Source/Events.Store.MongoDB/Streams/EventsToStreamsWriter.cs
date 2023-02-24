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
using StreamEvent = Dolittle.Runtime.Events.Store.MongoDB.Events.StreamEvent;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

/// <summary>
/// Represents an implementation of <see cref="IWriteEventsToStreams" /> and <see cref="IWriteEventsToStreamCollection" />..
/// </summary>
[DisableAutoRegistration]
public class EventsToStreamsWriter : IWriteEventsToStreamCollection, IWriteEventsToStreams
{
    readonly IStreams _streams;
    readonly IEventConverter _eventConverter;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventsToStreamsWriter"/> class.
    /// </summary>
    /// <param name="streams">The <see cref="IStreams"/>.</param>
    /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
    /// <param name="logger">An <see cref="ILogger"/>.</param>
    public EventsToStreamsWriter(IStreams streams, IEventConverter eventConverter, ILogger logger)
    {
        _streams = streams;
        _eventConverter = eventConverter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task Write(CommittedEvent @event, ScopeId scope, StreamId stream, PartitionId partition, CancellationToken cancellationToken)
        => Write(new []{(@event, partition)}, scope, stream, cancellationToken);

    /// <inheritdoc />
    public async Task Write(IEnumerable<(CommittedEvent, PartitionId)> events, ScopeId scope, StreamId stream, CancellationToken cancellationToken)
    {
        var lastWrittenStreamPosition = await Write(
            await _streams.Get(scope, stream, cancellationToken).ConfigureAwait(false),
            streamPosition =>
            {
                return events
                    .Select((eventAndPartition, i) =>
                        eventAndPartition.Item1 is CommittedExternalEvent externalEvent
                            ? _eventConverter.ToStoreStreamEvent(externalEvent, streamPosition + (ulong)i, eventAndPartition.Item2)
                            : _eventConverter.ToStoreStreamEvent(eventAndPartition.Item1, streamPosition + (ulong)i, eventAndPartition.Item2))
                    .ToList();
            },
            cancellationToken).ConfigureAwait(false);
        // _streamWatcher.NotifyForEvent(scope, stream, lastWrittenStreamPosition);
    }

    /// <inheritdoc/>
    public Task<StreamPosition> Write<TEvent>(IMongoCollection<TEvent> stream, Func<StreamPosition, TEvent> createStoreEvent, CancellationToken cancellationToken)
        => WriteOnlyNewEvents(stream, position => new List<TEvent>{createStoreEvent(position)}, cancellationToken);

    /// <inheritdoc />
    public Task<StreamPosition> Write<TEvent>(IMongoCollection<TEvent> stream, Func<StreamPosition, IReadOnlyList<TEvent>> createStoreEvents, CancellationToken cancellationToken)
        => WriteOnlyNewEvents(stream, createStoreEvents, cancellationToken);

    async Task<StreamPosition> WriteOnlyNewEvents<TEvent>(IMongoCollection<TEvent> stream, Func<StreamPosition, IReadOnlyList<TEvent>> createEventsToWrite, CancellationToken cancellationToken)
    {
        Task WriteToCollection(IReadOnlyList<TEvent> events, IClientSessionHandle transaction, CancellationToken cancellationToken) => events.Count switch
        {
            0 => Task.CompletedTask,
            1 => stream.InsertOneAsync(transaction, events[0], cancellationToken: cancellationToken),
            _ => stream.InsertManyAsync(transaction, events, cancellationToken: cancellationToken)
        };

        try
        {
            using var session = await _streams.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            try
            {
                session.StartTransaction();
                // TODO: Have a look at this.
                var streamPosition = (ulong) await stream.CountDocumentsAsync(
                    session,
                    Builders<TEvent>.Filter.Empty,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
                var eventsToWrite = createEventsToWrite(streamPosition).ToList();
                await WriteToCollection(eventsToWrite, session, cancellationToken).ConfigureAwait(false);
                await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                return streamPosition + (ulong) (eventsToWrite.Count - 1);
            }
            catch
            {
                await session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
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
                //TODO: Have a look at this.
                var streamPosition = (ulong) await stream.CountDocumentsAsync(
                    session,
                    Builders<TEvent>.Filter.Empty,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
                var eventsToWrite = createEventsToWrite(streamPosition).ToList();
                var streamEventsToWrite = eventsToWrite.Cast<Events.StreamEvent>().ToList();
                var storedEvents = await GetStoredEventStreamHeadOfStreamToWrite((IMongoCollection<Events.StreamEvent>) stream, streamEventsToWrite, session, cancellationToken).ConfigureAwait(false);
        
                if (!CheckIfStoredEventsIsPrefixOfEventsToWrite(streamEventsToWrite, storedEvents))
                {
                    throw;
                }
        
                eventsToWrite = eventsToWrite.Skip(storedEvents.Count).ToList();
                await WriteToCollection(eventsToWrite, session, cancellationToken).ConfigureAwait(false);
        
                await session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                return streamPosition + (ulong) (eventsToWrite.Count - 1);
            }
            catch
            {
                await session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }
    }

    
    
    static Task<List<Events.StreamEvent>> GetStoredEventStreamHeadOfStreamToWrite(IMongoCollection<Events.StreamEvent> stream, IReadOnlyList<Events.StreamEvent> eventsToWrite, IClientSessionHandle transaction, CancellationToken cancellationToken)
        => stream
            .Find(
                transaction,
                Builders<Events.StreamEvent>.Filter.Gte(_ => _.Metadata.EventLogSequenceNumber, eventsToWrite[0].Metadata.EventLogSequenceNumber))
            .SortBy(_ => _.Metadata.EventLogSequenceNumber)
            .ToListAsync(cancellationToken);

    static bool CheckIfStoredEventsIsPrefixOfEventsToWrite(IReadOnlyList<Events.StreamEvent> eventsToWrite, IReadOnlyList<Events.StreamEvent> storedEvents)
    {
        if (storedEvents.Count == 0)
        {
            return true;
        }

        if (storedEvents.Count > eventsToWrite.Count)
        {
            return false;
        }
        
        ThrowIfStoredEventsIsUnOrdered(storedEvents);
        for (var i = 0; i < storedEvents.Count; i++)
        {
            if (!IsSameStreamEvents(eventsToWrite[i], storedEvents[i]))
            {
                return false;
            }
        }

        return true;
    }

    static void ThrowIfStoredEventsIsUnOrdered(IReadOnlyList<StreamEvent> storedEvents)
    {
        for (var i = 0; i < storedEvents.Count - 1; i++)
        {
            if (storedEvents[i + 1].Metadata.EventLogSequenceNumber < storedEvents[i].Metadata.EventLogSequenceNumber)
            {
                throw new StoredStreamEventsHeadIsNotOrdered();
            }
        }
    }

    static bool IsSameStreamEvents(Events.StreamEvent eventToWrite, Events.StreamEvent storedEvent)
        => eventToWrite.Metadata.EventLogSequenceNumber.Equals(storedEvent.Metadata.EventLogSequenceNumber)
            && eventToWrite.Partition.Equals(storedEvent.Partition);

    static bool IsDuplicateKeyException(Exception exception)
        => exception switch
        {
            MongoDuplicateKeyException => true,
            MongoWriteException writeException => writeException.Message.Contains("duplicate key error"),
            MongoBulkWriteException bulkWriteException => bulkWriteException.Message.Contains("duplicate key error"),
            _ => false,
        };
}
