// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
    readonly IStreamProcessorLifecycleHooks _processorShutdownHook;
    readonly IApplicationLifecycleHooks _shutdownHook;
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger<ActorSystemClusterHostedService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActorSystemClusterHostedService"/>;
    /// </summary>
    /// <param name="actorSystem">The <see cref="ActorSystem"/>.</param>
    /// <param name="processorShutdownHook"></param>
    /// <param name="shutdownHook"></param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
    public ActorSystemClusterHostedService(ActorSystem actorSystem, IStreamProcessorLifecycleHooks processorShutdownHook,
        IApplicationLifecycleHooks shutdownHook, ILoggerFactory loggerFactory)
    {
        _actorSystem = actorSystem;
        _processorShutdownHook = processorShutdownHook;
        _shutdownHook = shutdownHook;
        _logger = loggerFactory.CreateLogger<ActorSystemClusterHostedService>();
        _loggerFactory = new ProtoInternalsLoggerFactoryProxy(loggerFactory);
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Log.SetLoggerFactory(_loggerFactory);
        await _actorSystem.Cluster().StartMemberAsync();
        _logger.LogInformation("Actor system started");
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Stop processing first
            await _processorShutdownHook.ShutdownGracefully(cancellationToken);
            // Then stops the event store / supporting processes 
            await _shutdownHook.ShutdownGracefully(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "An error occurred while shutting down shutdown hooks");
        }
        finally
        {
            await _actorSystem.Cluster().ShutdownAsync();
        }
    }
}
