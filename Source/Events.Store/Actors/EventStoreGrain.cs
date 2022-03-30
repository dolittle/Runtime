// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;
using Proto.Logging;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Events.Store.Actors;

[Singleton, PerTenant, Grain(typeof(EventStoreGrainActor))]
public class EventStoreGrain : EventStoreGrainBase
{
    readonly ClusterIdentity _identity;
    readonly Func<TenantId, IEventStore> _getEventStore;
    readonly ILogger _logger;

    public EventStoreGrain(IContext context, ClusterIdentity identity, Func<TenantId, IEventStore> getEventStore, ILogger logger)
        : base(context)
    {
        _identity = identity;
        _getEventStore = getEventStore;
        logger.LogInformation($"Created actor {identity}");
    }
    // public EventStoreGrain(IContext context, ClusterIdentity identity)
    //     : base(context)
    // {
    //     _identity = identity;
    //     // _getEventStore = getEventStore;
    //     Console.WriteLine($"Created actor {identity}");
    // }

    public override Task<CommitEventsResponse> Commit(CommitEventsRequest request)
        => throw new NotImplementedException();

    // public override Task<CommitAggregateEventsResponse> CommitForAggregate(CommitAggregateEventsRequest request)
    //     => throw new NotImplementedException();
    //
    // public override Task<FetchForAggregateResponse> FetchForAggregate(FetchForAggregateRequest request)
    //     => throw new NotImplementedException();
}
