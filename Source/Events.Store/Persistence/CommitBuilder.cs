// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Persistence;

/// <summary>
/// Represents a builder for a <see cref="Commit"/>.
/// </summary>
public class CommitBuilder
{
    readonly List<CommittedEvents> _committedEvents = new();
    readonly List<CommittedAggregateEvents> _committedAggregateEvents = new();
    readonly Dictionary<Aggregate, AggregateRootVersionRange> _aggregates = new();
    EventLogSequenceNumber _nextSequenceNumber;
    readonly List<CommittedEvent> _orderedEvents = new();
    readonly EventLogSequenceNumber _initialSequenceNumber;

    public IReadOnlyList<CommittedEvent> AllEvents => _orderedEvents;
    public int Count => _orderedEvents.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommitBuilder"/> class.
    /// </summary>
    /// <param name="nextSequenceNumber">The next <see cref="EventLogSequenceNumber"/>.</param>
    public CommitBuilder(EventLogSequenceNumber nextSequenceNumber)
    {
        _initialSequenceNumber = nextSequenceNumber;
        _nextSequenceNumber = nextSequenceNumber;
    }

    /// <summary>
    /// Gets a value indicating whether the commit has events.
    /// </summary>
    public bool HasCommits => _committedEvents.Count > 0 || _committedAggregateEvents.Count > 0;

    /// <summary>
    /// Try to add unto the <see cref="Commit"/> the events from a <see cref="CommitEventsRequest"/>.
    /// </summary>
    /// <param name="request">The <see cref="CommitEventsRequest"/>.</param>
    /// <returns>The <see cref="Try{TResult}"/> with the builder for continuation.</returns>
    public Try<CommittedEvents> TryAddEventsFrom(CommitEventsRequest request)
    {
        try
        {
            var nextSequenceNumber = _nextSequenceNumber;
            var executionContext = request.CallContext.ExecutionContext.ToExecutionContext();
            var occurred = DateTimeOffset.UtcNow;
            var committedEvents = new CommittedEvents(request.Events.Select(_ => new CommittedEvent(
                nextSequenceNumber++,
                occurred,
                _.EventSourceId,
                executionContext,
                _.EventType.ToArtifact(),
                _.Public,
                _.Content)).ToList());

            _committedEvents.Add(committedEvents);
            _orderedEvents.AddRange(committedEvents);
            _nextSequenceNumber = nextSequenceNumber;
            return committedEvents;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <summary>
    /// Try to add unto the <see cref="Commit"/> the events from a <see cref="CommitAggregateEventsRequest"/>.
    /// </summary>
    /// <param name="request">The <see cref="CommitAggregateEventsRequest"/>.</param>
    /// <returns>The <see cref="Try{TResult}"/> with the builder for continuation.</returns>
    public Try<CommittedAggregateEvents> TryAddEventsFrom(CommitAggregateEventsRequest request)
    {
        try
        {
            var nextSequenceNumber = _nextSequenceNumber;
            AggregateRootVersion nextAggregateRootVersion = request.Events.ExpectedAggregateRootVersion;
            var aggregate = new Aggregate(request.Events.AggregateRootId.ToGuid(), request.Events.EventSourceId);
            var aggregateRoot = new Artifact(aggregate.AggregateRoot, ArtifactGeneration.First);
            var executionContext = request.CallContext.ExecutionContext.ToExecutionContext();
            var occurred = DateTimeOffset.UtcNow;
            var committedEvents = new CommittedAggregateEvents(
                request.Events.EventSourceId,
                aggregate.AggregateRoot,
                request.Events.Events.Select(_ => new CommittedAggregateEvent(
                    aggregateRoot,
                    nextAggregateRootVersion++,
                    nextSequenceNumber++,
                    occurred,
                    request.Events.EventSourceId,
                    executionContext,
                    _.EventType.ToArtifact(),
                    _.Public,
                    _.Content)).ToList());

            if (!TryAddCommittedAggregateEvents(committedEvents, out var error))
            {
                return error;
            }

            _orderedEvents.AddRange(committedEvents);


            _nextSequenceNumber = nextSequenceNumber;
            return committedEvents;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <summary>
    /// Builds the <see cref="Commit"/> with the next <see cref="EventLogSequenceNumber"/>.
    /// </summary>
    /// <returns>A tuple of the built <see cref="Commit"/> and the next <see cref="EventLogSequenceNumber"/>.</returns>
    public (Commit Commit, EventLogSequenceNumber NextSequenceNumber) Build()
        => (new Commit(_committedEvents, _committedAggregateEvents, _orderedEvents, _initialSequenceNumber, _nextSequenceNumber - 1), _nextSequenceNumber);

    bool TryAddCommittedAggregateEvents(CommittedAggregateEvents events, out Exception error)
    {
        error = default;
        var aggregate = new Aggregate(events.AggregateRoot, events.EventSource);
        if (_aggregates.ContainsKey(new Aggregate(events.AggregateRoot, events.EventSource)))
        {
            error = new EventsForAggregateAlreadyAddedToCommit(aggregate);
            return false;
        }

        _committedAggregateEvents.Add(events);
        return true;

        //TODO: Make more sophisticated logic for determining whether a commit for the same aggregate can be added to the commit.
        // if (_aggregates.TryGetValue(aggregate, out var aggregateRootVersionRange))
        // {
        //     if (nextSequenceNumber != aggregateRootVersionRange.Start || request.Events.ExpectedAggregateRootVersion != aggregateRootVersionRange.End)
        //     {
        //         return new AggregateRootConcurrencyConflict(aggregate.EventSourceId, aggregate.AggregateRoot, request.Events.ExpectedAggregateRootVersion, aggregateRootVersionRange.End);
        //     }
        // }
        //
        // //TODO: Update the aggregate root version range
        // _committedAggregateEvents.Add(committedEvents);
    }
}
