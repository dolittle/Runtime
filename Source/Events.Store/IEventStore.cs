// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Defines the functionality required for an EventStore implementation.
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Commits events from a <see cref="CommitEventsRequest"/> to the Event Log.
    /// </summary>
    /// <param name="request">The request containing the execution context and events to commit.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="CommitEventsResponse">result</see> of the commit.</returns>
    Task<CommitEventsResponse> CommitEvents(CommitEventsRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Commits aggregate events from a <see cref="CommitAggregateEvents"/> to the Event Log.
    /// </summary>
    /// <param name="request">The request containing the execution context, aggregate root instance and events to commit.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="CommitAggregateEventsRequest">result</see> of the commit.</returns>
    Task<CommitAggregateEventsResponse> CommitAggregateEvents(CommitAggregateEventsRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Fetches aggregate events from the Event Log for an aggregate given an <see cref="FetchForAggregateRequest"/>.
    /// </summary>
    /// <param name="request">The request containing the execution context and aggregate root instance to fetch events for.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="FetchForAggregateResponse">result</see> of the fetch.</returns>
    Task<FetchForAggregateResponse> FetchAggregateEvents(FetchForAggregateRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Fetches aggregate events in batches from the Event Log for an aggregate given an <see cref="FetchForAggregateInBatchesRequest"/>.
    /// </summary>
    /// <param name="eventSource">The event source of the aggregate to fetch events from.</param>
    /// <param name="aggregateRoot">The aggregate </param>
    /// <param name="eventTypes">The event types of the aggregate events to fetch.</param>
    /// <param name="tenant">The tenant fetching events for.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{TResult}"/> of all the <see cref="CommittedAggregateEvent">aggregate events</see>.</returns>
    Try<IAsyncEnumerable<CommittedAggregateEvent>> FetchAggregateEvents(EventSourceId eventSource, ArtifactId aggregateRoot, IEnumerable<Artifact> eventTypes, TenantId tenant, CancellationToken cancellationToken);
}
