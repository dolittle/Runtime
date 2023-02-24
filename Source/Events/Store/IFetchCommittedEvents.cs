// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Defines an interface for fetching committed events the Event Store.
/// </summary>
public interface IFetchCommittedEvents
{
    // TODO: Maybe move this?
    /// <summary>
    /// Fetches the next <see cref="EventLogSequenceNumber"/> to use to commit an event.
    /// </summary>
    /// <param name="scope">The <see cref="ScopeId"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the next <see cref="EventLogSequenceNumber"/> to use to commit an event.</returns>
    Task<EventLogSequenceNumber> FetchNextSequenceNumber(ScopeId scope, CancellationToken cancellationToken);

    /// <summary>
    /// TODO: This should be made into a streaming call.
    /// Fetches EventLog <see cref="CommittedEvent"/>s from a given offset
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId"/>.</param>
    /// <param name="from">EventLogSequenceNumber to read from (inclusive)</param>
    /// <param name="limit">Max number of events to retrieve</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns></returns>
    Task<CommittedEvents> FetchCommittedEvents(ScopeId scopeId, EventLogSequenceNumber from, int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Fetches the <see cref="CommittedAggregateEvents"/> applied to an Event Source by an Aggregate Root.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> identifying the Event Source.</param>
    /// <param name="aggregateRoot">The <see cref="ArtifactId"/> identifying the Aggregate Root.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="FetchCommittedEvents"/>.</returns>
    IAsyncEnumerable<CommittedAggregateEvents>
        FetchStreamForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot, CancellationToken cancellationToken);

    /// <summary>
    /// Fetches the <see cref="CommittedAggregateEvents"/> applied to an Event Source by an Aggregate Root, filtered by Event Types.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> identifying the Event Source.</param>
    /// <param name="aggregateRoot">The <see cref="ArtifactId"/> identifying the Aggregate Root.</param>
    /// <param name="eventTypes">The <see cref="IEnumerable{T}"/> of <see cref="ArtifactId"/> identified the event types that should be fetched.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of <see cref="FetchCommittedEvents"/>.</returns>
    IAsyncEnumerable<CommittedAggregateEvents> FetchStreamForAggregate(EventSourceId eventSource, ArtifactId aggregateRoot, IEnumerable<ArtifactId> eventTypes,
        CancellationToken cancellationToken);

    /// <summary>
    /// TODO: This should be made into a streaming call.
    /// Fetches all <see cref="CommittedAggregateEvent"/>s applied to an Event Source by an Aggregate Root after the given version.
    /// </summary>
    /// <param name="eventSource">The <see cref="EventSourceId"/> identifying the Event Source.</param>
    /// <param name="aggregateRoot">The <see cref="ArtifactId"/> identifying the Aggregate Root.</param>
    /// <param name="after">The <see cref="AggregateRootVersion"/> to fetch events after.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="CommittedAggregateEvents"/> containing all <see cref="CommittedAggregateEvent"/> applied to the Event Source by the Aggregate root, in the order of which they appear in the Event Log.</returns>
    Task<CommittedAggregateEvents> FetchForAggregateAfter(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion after,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get the number of events matching the filter before and including the given <see cref="EventLogSequenceNumber"/>.
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="to"></param>
    /// <param name="eventTypes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<StreamPosition> GetStreamPositionFromArtifactSet(ScopeId scope, EventLogSequenceNumber to, IEnumerable<ArtifactId> eventTypes,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get the number of events matching the filter before and including the given <see cref="EventLogSequenceNumber"/>.
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="nextStreamPosition"></param>
    /// <param name="eventTypes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Try<EventLogSequenceNumber>> GetEventLogSequenceFromArtifactSet(ScopeId scope, StreamPosition nextStreamPosition, IEnumerable<ArtifactId> eventTypes,
        CancellationToken cancellationToken);
}
