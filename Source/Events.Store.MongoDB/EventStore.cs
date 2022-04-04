// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using Dolittle.Runtime.Events.Store.MongoDB.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Driver;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Store.MongoDB;

/// <summary>
/// Represents the MongoDB implementation of <see cref="IEventStore"/>.
/// </summary>
[PerTenant] //TODO: Singleton?
public class EventStore : IEventStore
{
    readonly FilterDefinitionBuilder<Events.Event> _eventFilter = Builders<Events.Event>.Filter;
    readonly IStreams _streams;
    readonly IEventCommitter _eventCommitter;
    readonly IEventConverter _eventConverter;
    readonly IAggregateRoots _aggregateRoots;
    readonly IStreamEventWatcher _streamWatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStore"/> class.
    /// </summary>
    /// <param name="streams">The <see cref="IStreams"/>.</param>
    /// <param name="eventCommitter">The <see cref="IEventCommitter" />.</param>
    /// <param name="eventConverter">The <see cref="IEventConverter" />.</param>
    /// <param name="aggregateRoots">The <see cref="IAggregateRoots" />.</param>
    /// <param name="streamWatcher">The <see cref="IStreamEventWatcher" />.</param>
    public EventStore(
        IStreams streams,
        IEventCommitter eventCommitter,
        IEventConverter eventConverter,
        IAggregateRoots aggregateRoots,
        IStreamEventWatcher streamWatcher)
    {
        _streams = streams;
        _eventCommitter = eventCommitter;
        _eventConverter = eventConverter;
        _aggregateRoots = aggregateRoots;
        _streamWatcher = streamWatcher;
    }

    /// <inheritdoc/>
    public async Task<CommittedEvents> CommitEvents(UncommittedEvents events, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        ThrowIfNoEventsToCommit(events);
        return await DoCommit<CommittedEvents, CommittedEvent>(async (transaction, cancel) =>
        {
            var eventLogSequenceNumber = (ulong)await _streams.DefaultEventLog.CountDocumentsAsync(
                transaction,
                _eventFilter.Empty,
                cancellationToken: cancel).ConfigureAwait(false);
            var committedEvents = new List<CommittedEvent>();
            foreach (var @event in events)
            {
                var committedEvent = await _eventCommitter.CommitEvent(
                    transaction,
                    eventLogSequenceNumber,
                    DateTimeOffset.UtcNow,
                    executionContext,
                    @event,
                    cancel).ConfigureAwait(false);
                committedEvents.Add(committedEvent);
                eventLogSequenceNumber++;
            }
            return new CommittedEvents(committedEvents);
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<CommittedAggregateEvents> CommitAggregateEvents(UncommittedAggregateEvents events, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        ThrowIfNoEventsToCommit(events);
        return await DoCommit<CommittedAggregateEvents, CommittedAggregateEvent>(async (transaction, cancel) =>
        {
            var eventLogSequenceNumber = (ulong)await _streams.DefaultEventLog.CountDocumentsAsync(
                transaction,
                _eventFilter.Empty,
                cancellationToken: cancel).ConfigureAwait(false);
            var aggregateRootVersion = events.ExpectedAggregateRootVersion.Value;

            var committedEvents = new List<CommittedAggregateEvent>();

            foreach (var @event in events)
            {
                var committedEvent = await _eventCommitter.CommitAggregateEvent(
                    transaction,
                    events.AggregateRoot,
                    aggregateRootVersion,
                    eventLogSequenceNumber,
                    DateTimeOffset.UtcNow,
                    events.EventSource,
                    executionContext,
                    @event,
                    cancel).ConfigureAwait(false);
                committedEvents.Add(committedEvent);
                eventLogSequenceNumber++;
                aggregateRootVersion++;
            }

            await _aggregateRoots.IncrementVersionFor(
                transaction,
                events.EventSource,
                events.AggregateRoot.Id,
                events.ExpectedAggregateRootVersion,
                aggregateRootVersion,
                cancel).ConfigureAwait(false);

            return new CommittedAggregateEvents(events.EventSource, events.AggregateRoot.Id, committedEvents);
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<CommittedAggregateEvents> FetchForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot, CancellationToken cancellationToken)
        => DoFetchForAggregate(eventSource, aggregateRoot, defaultFilter => defaultFilter, cancellationToken);

    /// <inheritdoc/>
    public Task<CommittedAggregateEvents> FetchForAggregateAfter(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion after, CancellationToken cancellationToken)
        => DoFetchForAggregate(eventSource, aggregateRoot, defaultFilter => defaultFilter & _eventFilter.Gt(_ => _.Aggregate.Version, after.Value), cancellationToken);

    Task<CommittedAggregateEvents> DoFetchForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot, Func<FilterDefinition<MongoDB.Events.Event>,FilterDefinition<MongoDB.Events.Event>> filterCallback, CancellationToken cancellationToken)
        => DoInSession<CommittedAggregateEvents, CommittedAggregateEvent>(async (transaction, cancel) =>
        {
            var version = await _aggregateRoots.FetchVersionFor(
                transaction,
                eventSource,
                aggregateRoot,
                cancel).ConfigureAwait(false);
            if (version > AggregateRootVersion.Initial)
            {
                var defaultFilter = _eventFilter.Eq(_ => _.Aggregate.WasAppliedByAggregate, true)
                    & _eventFilter.EqStringOrGuid(_ => _.Metadata.EventSource, eventSource.Value)
                    & _eventFilter.Eq(_ => _.Aggregate.TypeId, aggregateRoot.Value)
                    & _eventFilter.Lte(_ => _.Aggregate.Version, version.Value);

                var filter = filterCallback(defaultFilter);

                var events = await _streams.DefaultEventLog
                    .Find(transaction, filter)
                    .Sort(Builders<MongoDB.Events.Event>.Sort.Ascending(_ => _.Aggregate.Version))
                    .ToListAsync(cancel).ConfigureAwait(false);

                var aggregateEvents = events
                    .Select(_ => _eventConverter.ToRuntimeStreamEvent(_))
                    .Select(_ => _.Event)
                    .Cast<CommittedAggregateEvent>()
                    .ToList();

                return new CommittedAggregateEvents(
                    eventSource,
                    aggregateRoot,
                    aggregateEvents);
            }
            else
            {
                return new CommittedAggregateEvents(
                    eventSource,
                    aggregateRoot,
                    Array.Empty<CommittedAggregateEvent>());
            }
        }, cancellationToken);

    async Task<TCommittedEvents> DoInSession<TCommittedEvents, TEvent>(Func<IClientSessionHandle, CancellationToken, Task<TCommittedEvents>> doTask, CancellationToken cancellationToken)
        where TCommittedEvents : CommittedEventSequence<TEvent>
        where TEvent : CommittedEvent
    {
        try
        {
            using var session = await _streams.StartSessionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            return await session.WithTransactionAsync(doTask, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (MongoWaitQueueFullException ex)
        {
            throw new EventStoreUnavailable("Mongo wait queue is full", ex);
        }
    }

    async Task<TCommittedEvents> DoCommit<TCommittedEvents, TEvent>(Func<IClientSessionHandle, CancellationToken, Task<TCommittedEvents>> doTask, CancellationToken cancellationToken)
        where TCommittedEvents : CommittedEventSequence<TEvent>
        where TEvent : CommittedEvent
    {
        var committedEvents = await DoInSession<TCommittedEvents, TEvent>(doTask, cancellationToken).ConfigureAwait(false);
        if (committedEvents.HasEvents)
        {
            _streamWatcher.NotifyForEvent(ScopeId.Default, StreamId.EventLog, committedEvents.Max(_ => _.EventLogSequenceNumber.Value));
        }
        return committedEvents;
    }

    static void ThrowIfNoEventsToCommit(UncommittedEvents events)
    {
        if (!events.HasEvents)
        {
            throw new NoEventsToCommit();
        }
    }
}
