// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using Dolittle.Runtime.Events.Store.MongoDB.Projections;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.MongoDB;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using IAggregateRoots = Dolittle.Runtime.Events.Store.MongoDB.Aggregates.IAggregateRoots;

namespace Dolittle.Runtime.Events.Store.MongoDB;

/// <summary>
/// Represents the MongoDB implementation of <see cref="IEventStore"/>.
/// </summary>
[PerTenant]
public class CommittedEventsFetcher : IFetchCommittedEvents
{
    static readonly FilterDefinitionBuilder<Events.Event> _eventFilter = Builders<Events.Event>.Filter;
    static readonly SortDefinition<Events.Event> _eventLogSeqSortAsc = Builders<MongoDB.Events.Event>.Sort.Ascending(it => it.EventLogSequenceNumber);
    static readonly SortDefinition<Events.Event> _eventLogSeqSortDesc = Builders<MongoDB.Events.Event>.Sort.Descending(it => it.EventLogSequenceNumber);
    static readonly SortDefinition<Events.Event> _aggregateVersionSortAsc = Builders<MongoDB.Events.Event>.Sort.Ascending(it => it.Aggregate!.Version);

    readonly IStreams _streams;
    readonly IEventConverter _eventConverter;
    readonly IAggregateRoots _aggregateRoots;
    readonly ILogger<CommittedEventsFetcher> _logger;
    readonly IEventContentConverter _contentConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommittedEventsFetcher"/> class.
    /// </summary>
    /// <param name="streams">The <see cref="IStreams"/>.</param>
    /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots" />.</param>
    /// <param name="contentConverter">The <see cref="IEventContentConverter" />.</param>
    /// <param name="logger"></param>
    public CommittedEventsFetcher(
        IStreams streams,
        IEventConverter eventConverter,
        IAggregateRoots aggregateRoots,
        IEventContentConverter contentConverter,
        ILogger<CommittedEventsFetcher> logger)
    {
        _streams = streams;
        _eventConverter = eventConverter;
        _aggregateRoots = aggregateRoots;
        _contentConverter = contentConverter;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<EventLogSequenceNumber> FetchNextSequenceNumber(ScopeId scope, CancellationToken cancellationToken)
    {
        var eventLog = await GetEventLog(scope, cancellationToken).ConfigureAwait(false);
        var eventCount = (ulong)await eventLog.CountDocumentsAsync(
                _eventFilter.Empty,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (eventCount == 0) return 0ul; // No events means no need to double check

        var lastEvent = await eventLog
            .Find(_eventFilter.Empty)
            .Sort(_eventLogSeqSortDesc)
            .Limit(1)
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (lastEvent is null)
        {
            // Should not be possible
            _logger.LogError("No last event found, but event count was {EventCount}", eventCount);
            return 0ul;
        }

        var nextSequenceNumber = lastEvent.EventLogSequenceNumber + 1;

        if (nextSequenceNumber != eventCount)
        {
            _logger.LogError("Last event sequence number was {LastEventSequenceNumber}, but event count was {EventCount}", lastEvent.EventLogSequenceNumber,
                eventCount);
        }

        return nextSequenceNumber;
    }

    public async Task<EventLogSequenceNumber> FetchNextSequenceNumberAfter(ScopeId scope, DateTimeOffset timestamp, CancellationToken cancellationToken)
    {
        var eventLog = await GetEventLog(scope, cancellationToken).ConfigureAwait(false);

        // Search from the end of the event log
        // Find first event that happened before the given timestamp
        var lastEvent = await eventLog
            .Find(_eventFilter.Lt(evt => evt.Metadata.Occurred, timestamp.UtcDateTime))
            .Sort(_eventLogSeqSortDesc)
            .Limit(1)
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (lastEvent is null)
        {
            return EventLogSequenceNumber.Initial;
        }

        // The next event will be after the timestamp cutoff
        return lastEvent.EventLogSequenceNumber + 1;
    }

    /// <inheritdoc/>
    public async Task<CommittedEvents> FetchCommittedEvents(ScopeId scope, EventLogSequenceNumber from, EventLogSequenceNumber to, int limit,
        ISet<Guid> artifactSet, CancellationToken cancellationToken)
    {
        try
        {
            var eventLog = await GetEventLog(scope, cancellationToken).ConfigureAwait(false);
            var filter = _eventFilter.Gte(e => e.EventLogSequenceNumber, from) & _eventFilter.Lt(e => e.EventLogSequenceNumber, to);
            if (artifactSet.Count > 0)
            {
                filter &= _eventFilter.In(_ => _.Metadata.TypeId, artifactSet);
            }

            var raw = await eventLog
                .Find(filter)
                .Sort(_eventLogSeqSortAsc)
                .Limit(limit)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            var events = raw.Select(evt => _eventConverter.ToRuntimeCommittedEvent(evt)).ToList();

            return new CommittedEvents(events);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    public IAsyncEnumerable<CommittedAggregateEvents> FetchStreamForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot,
        CancellationToken cancellationToken)
        => DoFetchForAggregate(eventSource, aggregateRoot, _eventFilter.Empty, cancellationToken);

    public IAsyncEnumerable<CommittedAggregateEvents> FetchStreamForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot,
        IEnumerable<ArtifactId> eventTypes, CancellationToken cancellationToken)
        => DoFetchForAggregate(eventSource, aggregateRoot, _eventFilter.In(_ => _.Metadata.TypeId, eventTypes.Select(_ => _.Value)), cancellationToken);

    /// <inheritdoc/>
    public async Task<CommittedAggregateEvents> FetchForAggregateAfter(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion after,
        CancellationToken cancellationToken)
    {
        var stream = DoFetchForAggregate(eventSource, aggregateRoot, _eventFilter.Gt(_ => _.Aggregate.Version, after.Value), cancellationToken);
        var events = await stream.ToListAsync(cancellationToken).ConfigureAwait(false);

        return new CommittedAggregateEvents(
            events[0].EventSource,
            events[0].AggregateRoot,
            events[0].AggregateRootVersion,
            events.SelectMany(_ => _).ToList());
    }

    async IAsyncEnumerable<CommittedAggregateEvents> DoFetchForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot,
        FilterDefinition<MongoDB.Events.Event> filter, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        AggregateRootVersion version;
        try
        {
            version = await _aggregateRoots.FetchVersionFor(eventSource, aggregateRoot, cancellationToken).ConfigureAwait(false);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
        catch (Exception ex)
        {
            throw new EventStoreUnavailable("Unable to fetch version", ex);
        }

        if (version <= AggregateRootVersion.Initial)
        {
            yield return EmptyCommittedAggregateEvents(eventSource, aggregateRoot, version);
            yield break;
        }

        IAsyncEnumerable<CommittedAggregateEvent> stream;
        try
        {
            stream = _streams.DefaultEventLog
                .Find(EventsFromAggregateFilter(eventSource, aggregateRoot, version) & filter)
                .Sort(_aggregateVersionSortAsc)
                .Project(evt => new AggregateEventProjection
                {
                    EventLogSequenceNumber = evt.EventLogSequenceNumber,
                    Content = evt.Content,
                    ExecutionContext = evt.ExecutionContext,
                    Metadata = evt.Metadata,
                    Aggregate = evt.Aggregate,
                })
                .ToAsyncEnumerable(cancellationToken)
                .Select(evt => new CommittedAggregateEvent(
                    new Artifact(
                        evt.Aggregate.TypeId,
                        evt.Aggregate.TypeGeneration),
                    aggregateRootVersion: evt.Aggregate.Version,
                    eventLogSequenceNumber: evt.EventLogSequenceNumber,
                    occurred:evt.Metadata.Occurred,
                    eventSource:evt.Metadata.EventSource,
                    evt.ExecutionContext.ToExecutionContext(),
                    new Artifact(
                        evt.Metadata.TypeId,
                        evt.Metadata.TypeGeneration),
                    evt.Metadata.Public,
                    _contentConverter.ToJson(evt.Content)));
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
        catch (Exception ex)
        {
            throw new EventStoreUnavailable("Failed to get aggregate events", ex);
        }

        var hasEvents = false;
        await foreach (var batch in stream
            .WithCancellation(cancellationToken))
        {
            hasEvents = true;
            var response = new CommittedAggregateEvents(
                eventSource,
                aggregateRoot,
                version,
                new[] { batch });
            yield return response;
        }

        if (!hasEvents)
        {
            yield return EmptyCommittedAggregateEvents(eventSource, aggregateRoot, version);
        }
    }

    static CommittedAggregateEvents EmptyCommittedAggregateEvents(EventSourceId eventSource, ArtifactId aggregateRoot,
        AggregateRootVersion aggregateRootVersion)
        => new(eventSource, aggregateRoot, aggregateRootVersion, Array.Empty<CommittedAggregateEvent>());

    FilterDefinition<Events.Event> EventsFromAggregateFilter(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion aggregateRootVersion)
        => _eventFilter.Eq(_ => _.Aggregate.WasAppliedByAggregate, true)
           & _eventFilter.Eq(_ => _.Aggregate.TypeId, aggregateRoot.Value)
           & _eventFilter.EqStringOrGuid(_ => _.Metadata.EventSource, eventSource.Value)
           & _eventFilter.Lte(_ => _.Aggregate.Version, aggregateRootVersion.Value);

    Task<IMongoCollection<MongoDB.Events.Event>> GetEventLog(ScopeId scope, CancellationToken cancellationToken)
        => scope == ScopeId.Default
            ? Task.FromResult(_streams.DefaultEventLog)
            : _streams.GetEventLog(scope, cancellationToken);
}
