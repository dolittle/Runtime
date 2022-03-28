// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.Server.Actors;

public class ActorSystemClusterHostedService : IHostedService
{
    readonly ActorSystem _actorSystem;
    readonly ILoggerFactory _loggerFactory;


    public ActorSystemClusterHostedService(ActorSystem actorSystem, ILoggerFactory loggerFactory)
    {
        _actorSystem = actorSystem;
        _loggerFactory = loggerFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Log.SetLoggerFactory(_loggerFactory);
        return _actorSystem.Cluster().StartMemberAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => _actorSystem.Cluster().ShutdownAsync();
}
