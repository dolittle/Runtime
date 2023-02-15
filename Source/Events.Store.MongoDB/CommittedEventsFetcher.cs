// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.MongoDB;
using Dolittle.Runtime.Rudimentary;
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
    readonly FilterDefinitionBuilder<Events.Event> _eventFilter = Builders<Events.Event>.Filter;
    readonly IStreams _streams;
    readonly IEventConverter _eventConverter;
    readonly IAggregateRoots _aggregateRoots;
    readonly ILogger<CommittedEventsFetcher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommittedEventsFetcher"/> class.
    /// </summary>
    /// <param name="streams">The <see cref="IStreams"/>.</param>
    /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots" />.</param>
    public CommittedEventsFetcher(
        IStreams streams,
        IEventConverter eventConverter,
        IAggregateRoots aggregateRoots, ILogger<CommittedEventsFetcher> logger)
    {
        _streams = streams;
        _eventConverter = eventConverter;
        _aggregateRoots = aggregateRoots;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<EventLogSequenceNumber> FetchNextSequenceNumber(ScopeId scope, CancellationToken cancellationToken)
    {
        var eventLog = await GetEventLog(scope, cancellationToken).ConfigureAwait(false);
        return (ulong)await eventLog.CountDocumentsAsync(
                _eventFilter.Empty,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        // TODO: This is for getting the sequence number from the last event.
        // var lastEvent = await eventLog
        //     .Find(_eventFilter.Empty)
        //     .Sort(Builders<MongoDB.Events.Event>.Sort.Descending(_ => _.EventLogSequenceNumber))
        //     .Limit(1)
        //     .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        //
        // return lastEvent?.EventLogSequenceNumber ?? 0ul;
    }

    /// <inheritdoc />
    public async Task<StreamPosition> GetStreamPositionFromArtifactSet(ScopeId scope, EventLogSequenceNumber to, IEnumerable<ArtifactId> eventTypes,
        CancellationToken cancellationToken)
    {
        var filter = _eventFilter.And(
            _eventFilter.Lte(_ => _.EventLogSequenceNumber, to.Value),
            ToMongoFilterDefinition(eventTypes)
        );
        var eventLog = await GetEventLog(scope, cancellationToken).ConfigureAwait(false);
        return (ulong)await eventLog.CountDocumentsAsync(
                filter,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<Try<EventLogSequenceNumber>> GetEventLogSequenceFromArtifactSet(ScopeId scope, StreamPosition to, IEnumerable<ArtifactId> eventTypes,
        CancellationToken cancellationToken)
    {
        return Try<EventLogSequenceNumber>.DoAsync(async () =>
        {
            var filter = ToMongoFilterDefinition(eventTypes);
            var eventLog = await GetEventLog(scope, cancellationToken).ConfigureAwait(false);
            var value = Convert.ToInt32(to.Value);
            
            _logger.LogInformation("Getting event log sequence number from artifact set: {Filter}, {Value}, {EventLog}", filter, value, eventLog);
            
            
            var evt = await eventLog.Find(filter)
                .Sort(Builders<MongoDB.Events.Event>.Sort.Ascending(_ => _.EventLogSequenceNumber))
                .Limit(value)
                .Skip(value - 1)
                .FirstOrDefaultAsync(cancellationToken);

            return evt?.EventLogSequenceNumber ?? throw new ArgumentException("Could not find event log sequence number");
        });
    }

    FilterDefinition<Events.Event> ToMongoFilterDefinition(IEnumerable<ArtifactId> artifactIds)
    {
        var eventTypes = artifactIds.Select(_ => _.Value).ToHashSet();
        return _eventFilter.In(_ => _.Metadata.TypeId, eventTypes);
    }

    /// <inheritdoc/>
    public async Task<CommittedEvents> FetchCommittedEvents(ScopeId scope, EventLogSequenceNumber from, int limit, CancellationToken cancellationToken)
    {
        try
        {
            var eventLog = await GetEventLog(scope, cancellationToken).ConfigureAwait(false);
            var events = await eventLog
                .Find(_eventFilter.Gte(e => e.EventLogSequenceNumber, from))
                .Sort(Builders<MongoDB.Events.Event>.Sort.Ascending(_ => _.EventLogSequenceNumber))
                .Limit(limit)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return new CommittedEvents(
                events.Select(_ => _eventConverter.ToRuntimeStreamEvent(_))
                    .Select(_ => _.Event)
                    .ToList());
        }
        catch (Exception ex)
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
        FilterDefinition<MongoDB.Events.Event> filter, CancellationToken cancellationToken)
    {
        AggregateRootVersion version;
        try
        {
            version = await _aggregateRoots.FetchVersionFor(eventSource, aggregateRoot, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }

        if (version <= AggregateRootVersion.Initial)
        {
            yield return EmptyCommittedAggregateEvents(eventSource, aggregateRoot, version);
            yield break;
        }

        IAsyncEnumerable<CommittedAggregateEvents> stream;
        try
        {
            stream = _streams.DefaultEventLog
                .Find(filter & EventsFromAggregateFilter(eventSource, aggregateRoot, version))
                .Sort(Builders<MongoDB.Events.Event>.Sort.Ascending(_ => _.Aggregate.Version))
                .ToAsyncEnumerable(cancellationToken)
                .Select(_ => new CommittedAggregateEvents(
                    eventSource,
                    aggregateRoot,
                    version,
                    new[] { (CommittedAggregateEvent)_eventConverter.ToRuntimeStreamEvent(_).Event }));
        }
        catch (Exception ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }

        var hasEvents = false;
        await foreach (var batch in stream.WithCancellation(cancellationToken))
        {
            hasEvents = true;
            yield return batch;
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
           & _eventFilter.EqStringOrGuid(_ => _.Metadata.EventSource, eventSource.Value)
           & _eventFilter.Eq(_ => _.Aggregate.TypeId, aggregateRoot.Value)
           & _eventFilter.Lte(_ => _.Aggregate.Version, aggregateRootVersion.Value);

    Task<IMongoCollection<MongoDB.Events.Event>> GetEventLog(ScopeId scope, CancellationToken cancellationToken)
        => scope == ScopeId.Default
            ? Task.FromResult(_streams.DefaultEventLog)
            : _streams.GetEventLog(scope, cancellationToken);
}
