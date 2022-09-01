// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.MongoDB;
using Dolittle.Runtime.Rudimentary;
using MongoDB.Driver;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="CommittedEventsFetcher"/> class.
    /// </summary>
    /// <param name="streams">The <see cref="IStreams"/>.</param>
    /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots" />.</param>
    public CommittedEventsFetcher(
        IStreams streams,
        IEventConverter eventConverter,
        IAggregateRoots aggregateRoots)
    {
        _streams = streams;
        _eventConverter = eventConverter;
        _aggregateRoots = aggregateRoots;
    }

    /// <inheritdoc />
    public async Task<EventLogSequenceNumber> FetchNextSequenceNumber(ScopeId scope, CancellationToken cancellationToken)
    {
        var eventLog = await GetEventLog(scope, cancellationToken).ConfigureAwait(false);
        return (ulong)await eventLog.CountDocumentsAsync(
                _eventFilter.Empty,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
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

    /// <inheritdoc/>
    public async Task<CommittedAggregateEvents> FetchForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot, CancellationToken cancellationToken)
    {
        var (aggregateRootVersion, eventStream) = await DoFetchForAggregate(
            eventSource,
            aggregateRoot,
            filter => filter,
            cancellationToken).ConfigureAwait(false);

        var events = await eventStream.ToListAsync(cancellationToken);
        if ((ulong) events.Count != aggregateRootVersion)
        {
            throw new InconsistentNumberOfCommittedAggregateEvents(eventSource, aggregateRoot, aggregateRootVersion, events.Count);
        }
        return new CommittedAggregateEvents(eventSource, aggregateRoot, aggregateRootVersion, events);
    }

    /// <inheritdoc/>
    public async Task<Try<(AggregateRootVersion AggregateRootVersion, IAsyncEnumerable<CommittedAggregateEvent> EventStream)>> FetchForAggregate(
        EventSourceId eventSource,
        ArtifactId aggregateRoot,
        IEnumerable<Artifact> eventTypes,
        CancellationToken cancellationToken)
    {
        try
        {
            return await DoFetchForAggregate(
                eventSource,
                aggregateRoot,
                filter => filter & _eventFilter.In(_ => _.Metadata.TypeId, eventTypes.Select(_ => _.Id.Value)),
                cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <inheritdoc/>
    public async Task<CommittedAggregateEvents> FetchForAggregateAfter(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion after, CancellationToken cancellationToken)
    {
        var (version, eventStream) = await DoFetchForAggregate(
            eventSource,
            aggregateRoot,
            filter => filter & _eventFilter.Gt(_ => _.Aggregate.Version, after.Value),
            cancellationToken).ConfigureAwait(false);

        return new CommittedAggregateEvents(eventSource, aggregateRoot, version, await eventStream.ToListAsync(cancellationToken).ConfigureAwait(false));
    }

    async Task<(AggregateRootVersion AggregateRootVersion, IAsyncEnumerable<CommittedAggregateEvent> EventStream)> DoFetchForAggregate(
        EventSourceId eventSource,
        ArtifactId aggregateRoot,
        Func<FilterDefinition<MongoDB.Events.Event>, FilterDefinition<MongoDB.Events.Event>> filterCallback,
        CancellationToken cancellationToken)
    {
        try
        {
            var version = await _aggregateRoots.FetchVersionFor(
                eventSource,
                aggregateRoot,
                cancellationToken).ConfigureAwait(false);
            if (version <= AggregateRootVersion.Initial)
            {
                return (version, Array.Empty<CommittedAggregateEvent>().ToAsyncEnumerable());
            }

            var filter = filterCallback(EventsFromAggregateFilter(eventSource, aggregateRoot, version));
            var eventStream = _streams.DefaultEventLog
                .Find(filter)
                .Sort(Builders<MongoDB.Events.Event>.Sort.Ascending(_ => _.Aggregate.Version))
                .ToAsyncEnumerable(cancellationToken)
                .Select(_ => (CommittedAggregateEvent) _eventConverter.ToRuntimeStreamEvent(_).Event);

            return (version, eventStream);
        }
        catch (Exception ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

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

