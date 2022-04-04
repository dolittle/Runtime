// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store;

public class EventStore : IEventStore
{
    readonly Func<TenantId, EventStoreClient> _getClient;

    public EventStore(Func<TenantId, EventStoreClient> getClient)
    {
        _getClient = getClient;
    }

    public Task<CommitEventsResponse> CommitEvents(CommitEventsRequest request, CancellationToken cancellationToken)
        => _getClient(request.CallContext.ExecutionContext.TenantId.ToGuid())
            .Commit(request, cancellationToken);

    public Task<CommitAggregateEventsResponse> CommitAggregateEvents(CommitAggregateEventsRequest request, CancellationToken cancellationToken)
        => _getClient(request.CallContext.ExecutionContext.TenantId.ToGuid())
            .CommitForAggregate(request, cancellationToken);

    public Task<FetchForAggregateResponse> FetchAggregateEvents(FetchForAggregateRequest request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
