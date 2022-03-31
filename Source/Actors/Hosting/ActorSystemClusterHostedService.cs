// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;

namespace Dolittle.Runtime.Actors.Hosting;

/// <summary>
/// Represents an implementation of <see cref="IHostedService"/> for the Proto.Actor Cluster.
/// </summary>
public class ActorSystemClusterHostedService : IHostedService
{
    readonly ActorSystem _actorSystem;
    readonly ILoggerFactory _loggerFactory;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ActorSystemClusterHostedService"/>;
    /// </summary>
    /// <param name="actorSystem">The <see cref="ActorSystem"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public ActorSystemClusterHostedService(ActorSystem actorSystem, ILoggerFactory loggerFactory)
    {
        _actorSystem = actorSystem;
        _loggerFactory = loggerFactory;
    }
    
    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Log.SetLoggerFactory(_loggerFactory);
        return _actorSystem.Cluster().StartMemberAsync();
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
        => _actorSystem.Cluster().ShutdownAsync();
}
