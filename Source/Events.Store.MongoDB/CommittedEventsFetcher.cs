 // Copyright (c) Dolittle. All rights reserved.
 // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
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
    public async Task<EventLogSequenceNumber> FetchNextSequenceNumber(CancellationToken cancellationToken)
    {
        return (ulong)await _streams.DefaultEventLog.CountDocumentsAsync(
                _eventFilter.Empty,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<CommittedAggregateEvents> FetchForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot, CancellationToken cancellationToken)
        => DoFetchForAggregate(
            eventSource,
            aggregateRoot,
            defaultFilter => defaultFilter,
            cancellationToken);

    /// <inheritdoc/>
    public Task<CommittedAggregateEvents> FetchForAggregateAfter(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion after, CancellationToken cancellationToken)
        => DoFetchForAggregate(
            eventSource,
            aggregateRoot,
            defaultFilter => defaultFilter & _eventFilter.Gt(_ => _.Aggregate.Version, after.Value),
            cancellationToken);

    async Task<CommittedAggregateEvents> DoFetchForAggregate(
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
                return new CommittedAggregateEvents(
                    eventSource,
                    aggregateRoot,
                    Array.Empty<CommittedAggregateEvent>());
            }
            var defaultFilter = _eventFilter.Eq(_ => _.Aggregate.WasAppliedByAggregate, true)
                & _eventFilter.EqStringOrGuid(_ => _.Metadata.EventSource, eventSource.Value)
                & _eventFilter.Eq(_ => _.Aggregate.TypeId, aggregateRoot.Value)
                & _eventFilter.Lte(_ => _.Aggregate.Version, version.Value);

            var filter = filterCallback(defaultFilter);

            var events = await _streams.DefaultEventLog
                .Find(filter)
                .Sort(Builders<MongoDB.Events.Event>.Sort.Ascending(_ => _.Aggregate.Version))
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return new CommittedAggregateEvents(
                eventSource,
                aggregateRoot,
                events.Select(_ => _eventConverter.ToRuntimeStreamEvent(_))
                    .Select(_ => _.Event)
                    .Cast<CommittedAggregateEvent>().ToList());
        }
        catch (Exception ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }
}
