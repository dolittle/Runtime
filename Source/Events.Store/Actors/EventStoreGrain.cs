// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.Events.Store.Actors;

public class EventStoreGrain : EventStoreGrainBase
{
    readonly ClusterIdentity _identity;
    readonly ILogger _logger;

    public EventStoreGrain(IContext context, ClusterIdentity identity, ILogger logger)
        : base(context)
    {
        _identity = identity;
        _logger = logger;
    }

    public override async Task Commit()
        => _logger.LogInformation($"Event Store {_identity.Identity} is committing");
}
