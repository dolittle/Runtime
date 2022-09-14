// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;

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
    /// Fetches aggregate events in batches from the Event Log for an aggregate given an <see cref="FetchForAggregateInBatchesRequest"/>.
    /// </summary>
    /// <param name="request">The <see cref="FetchEventsForAggregateInBatchesRequest"/> request.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the operation.</param>
    /// <returns><see cref="IAsyncEnumerable{TResult}"/> of <see cref="FetchForAggregateResponse"/> batched responses.</returns>
    IAsyncEnumerable<FetchForAggregateResponse> FetchAggregateEvents(FetchForAggregateInBatchesRequest request, CancellationToken cancellationToken);
}
