// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Defines a system that can commit an event.
/// </summary>
public interface IEventCommitter
{
    // TODO: Move and use Try?
    /// <summary>
    /// Persists <see cref="CommittedEvents"/> to the event log.
    /// </summary>
    /// <param name="transaction">The <see cref="IClientSessionHandle" />.</param>
    /// <param name="events">The <see cref="CommittedEvents"/> to persist.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task"/> that is resolved when the events are persisted.</returns>
    Task PersistEvents(
        IClientSessionHandle transaction,
        CommittedEvents events,
        CancellationToken cancellationToken);

    /// <summary>
    /// Commits a single <see cref="UncommittedEvent"/> applied to an event source by an aggregate root to the event log.
    /// </summary>
    /// <param name="transaction">The <see cref="IClientSessionHandle" />.</param>
    /// <param name="aggregateRoot">The <see cref="Artifact"/> identifying the type of the aggregate root that applied the event.</param>
    /// <param name="aggregateRootVersion">The <see cref="AggregateRootVersion"/> of the aggregate root that applied the event.</param>
    /// <param name="version">The expected next <see cref="EventLogSequenceNumber"/> of the event log.</param>
    /// <param name="occurred">The <see cref="DateTimeOffset"/> when the event occurred.</param>
    /// <param name="eventSource">The <see cref="EventSourceId"/> the event was applied to.</param>
    /// <param name="executionContext">The <see cref="Execution.ExecutionContext" />.</param>
    /// <param name="event">The <see cref="UncommittedEvent"/> to commit.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The <see cref="CommittedAggregateEvent" />.</returns>
    Task<CommittedAggregateEvent> CommitAggregateEvent(
        IClientSessionHandle transaction,
        Artifact aggregateRoot,
        AggregateRootVersion aggregateRootVersion,
        EventLogSequenceNumber version,
        DateTimeOffset occurred,
        EventSourceId eventSource,
        Execution.ExecutionContext executionContext,
        UncommittedEvent @event,
        CancellationToken cancellationToken);
}
