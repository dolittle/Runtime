// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Defines an interface for committing events.
/// </summary>
public interface ICommitEvents
{
    /// <summary>
    /// Persists events to the Event Store.
    /// </summary>
    /// <param name="events">The <see cref="CommittedEvents"/> to be committed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the result of the operation.</returns>
    Task<Try> Persist(CommittedEvents events, CancellationToken cancellationToken);

    /// <summary>
    /// Commits an <see cref="UncommittedAggregateEvents"/> to the Event Store, returning a corresponding <see cref="CommittedAggregateEvents"/>.
    /// When committing event to the Event Store using Aggregate Roots, concurrency is guaranteed scoped to Aggregate Root instances.
    /// </summary>
    /// <param name="events">The <see cref="UncommittedAggregateEvents"/> to be committed.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/> to commit events in.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="CommittedAggregateEvents"/> corresponding to the <see cref="UncommittedAggregateEvents"/> supplied.</returns>
    Task<CommittedAggregateEvents> CommitAggregateEvents(UncommittedAggregateEvents events, ExecutionContext executionContext, CancellationToken cancellationToken);
}
