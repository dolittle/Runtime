// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Services.Hosting;
using Grpc.Core;
using static Dolittle.Runtime.Events.Contracts.EventStore;

namespace Dolittle.Runtime.Events.Store.Services.Grpc;

/// <summary>
/// Represents the implementation of <see cref="EventStoreBase"/>.
/// </summary>
[PrivateService]
public class EventStoreGrpcService : EventStoreBase
{
    readonly IEventStore _eventStore;
    readonly IFetchCommittedEvents _committedEventsFetcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreGrpcService"/> class.
    /// </summary>
    /// <param name="eventStore">The event store to use.</param>
    /// <param name="committedEventsReader"></param>
    public EventStoreGrpcService(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    /// <inheritdoc/>
    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request, ServerCallContext context)
        => _eventStore.CommitEvents(request, context.CancellationToken);

    /// <inheritdoc/>
    public override Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request, ServerCallContext context)
        => _eventStore.CommitAggregateEvents(request, context.CancellationToken);

    /// <inheritdoc/>
    public override Task<FetchForAggregateResponse> FetchForAggregate(FetchForAggregateRequest request, ServerCallContext context)
        => _eventStore.FetchAggregateEvents(request, context.CancellationToken);
}
