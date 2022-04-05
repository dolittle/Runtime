// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Contracts;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

public class EventStore : EventStoreBase
{
    public EventStore(IContext context)
        : base(context)
    {
    }

    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request)
        => throw new System.NotImplementedException();

    public override Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request)
        => throw new System.NotImplementedException();
}
