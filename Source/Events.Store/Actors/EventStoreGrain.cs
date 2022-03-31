// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.Events.Store.Actors;

[TenantGrain(typeof(EventStoreGrainActor), typeof(EventStoreGrainClient))]
public class EventStoreGrain : EventStoreGrainBase
{
    readonly ClusterIdentity _identity;
    readonly IEventStore _eventStore;
    readonly ILogger _logger;

    public EventStoreGrain(IContext context, ClusterIdentity identity, IEventStore eventStore, TenantId tenantId, ILogger logger)
        : base(context)
    {
        _identity = identity;
        _eventStore = eventStore;
        _logger = logger;
        logger.LogInformation($"Created actor {identity}");
    }

    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request)
        => throw new NotImplementedException();
}
