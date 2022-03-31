// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Rudimentary;
using DolittleExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Store.Services;

/// <summary>
/// Defines the service that commits events to the event store.
/// </summary>
public interface IEventStoreService
{
    /// <summary>
    /// Try to commit the <see cref="UncommittedEvents" />.
    /// </summary>
    /// <param name="events"><see cref="UncommittedEvents" /> to commit.</param>
    /// <param name="context"><see cref="DolittleExecutionContext" /> the <see cref="UncommittedEvents" /> are committed in.</param>
    /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of the <see cref="CommittedEvents" />.</returns>
    Task<Try<CommittedEvents>> TryCommit(UncommittedEvents events, DolittleExecutionContext context, CancellationToken token);
    
    /// <summary>
    /// Performs a commit.
    /// </summary>
    /// <param name="request">The <see cref="CommitEventsRequest"/>.</param>
    /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns  <see cref="CommitEventsResponse" />.</returns>
    Task<CommitEventsResponse> Commit(CommitEventsRequest request, CancellationToken token);

    /// <summary>
    /// Try to commit the <see cref="UncommittedAggregateEvents" />.
    /// </summary>
    /// <param name="events"><see cref="UncommittedAggregateEvents" /> to commit.</param>
    /// <param name="context"><see cref="DolittleExecutionContext" /> the <see cref="UncommittedAggregateEvents" /> are committed in.</param>
    /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of the <see cref="CommittedAggregateEvents" />.</returns>
    Task<Try<CommittedAggregateEvents>> TryCommitForAggregate(UncommittedAggregateEvents events, DolittleExecutionContext context, CancellationToken token);

    /// <summary>
    /// Performs a commit for aggregate.
    /// </summary>
    /// <param name="request">The <see cref="CommitAggregateEventsRequest"/>.</param>
    /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns  <see cref="CommitAggregateEventsResponse" />.</returns>
    Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request, CancellationToken token);
    
    /// <summary>
    /// Try to fetch events for a specific aggregate.
    /// </summary>
    /// <param name="aggregateRoot">The <see cref="ArtifactId" /> of the aggregate root to fetch events from.</param>
    /// <param name="eventSource">The <see cref="EventSourceId" /> of the specific aggregate.</param>
    /// <param name="context">The <see cref="DolittleExecutionContext" /> to fetch the <see cref="CommittedAggregateEvents" /> in.</param>
    /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of the <see cref="CommittedAggregateEvents" />.</returns>
    Task<Try<CommittedAggregateEvents>> TryFetchForAggregate(ArtifactId aggregateRoot, EventSourceId eventSource, DolittleExecutionContext context, CancellationToken token);
    
    /// <summary>
    /// Performs fetch for aggregate.
    /// </summary>
    /// <param name="request">The <see cref="FetchForAggregateRequest"/>.</param>
    /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns  <see cref="FetchForAggregateRequest" />.</returns>
    Task<FetchForAggregateResponse> FetchForAggregate(FetchForAggregateRequest request, CancellationToken token);
}
