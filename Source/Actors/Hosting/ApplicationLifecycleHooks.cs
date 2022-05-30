// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;

namespace Dolittle.Runtime.Actors.Hosting;

/// <summary>
/// Shutdown hooks allow the registrant to perform necessary cleanup actions on asynchronously on shutdown before marking the hook complete,
/// allowing the shutdown to continue
/// </summary>
public interface IShutdownHook
{
    /// <summary>
    /// Gets a <see cref="Task"/> that completes when shutdown is triggered.
    /// </summary>
    Task ShuttingDown { get; }

    /// <summary>
    /// Marks the shutdown.
    /// </summary>
    void MarkCompleted();
}

/// <summary>
/// Defines a system that knows about application lifecycle hooks.
/// </summary>
public interface IApplicationLifecycleHooks
{
    /// <summary>
    /// Register a shutdown hook.
    /// </summary>
    /// <returns>The registered <see cref="IShutdownHook"/>.</returns>
    IShutdownHook RegisterShutdownHook();
    
    /// <summary>
    /// Shutdown all the hooks gracefully.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> that is resolved when all <see cref="IShutdownHook"/> are completed.</returns>
    Task ShutdownGracefully(CancellationToken cancellationToken);
}

/// <summary>
/// Represents an implementation of <see cref="IApplicationLifecycleHooks"/>.
/// </summary>
[Singleton]
// ReSharper disable once ClassNeverInstantiated.Global
public class ApplicationLifecycleHooks : IApplicationLifecycleHooks
{
    int _i = 0;

    readonly TaskCompletionSource _shutdownSource = new();
    readonly ConcurrentDictionary<int, Task> _registered = new();
    
    /// <inheritdoc />
    public IShutdownHook RegisterShutdownHook()
    {
        var id = Interlocked.Increment(ref _i);
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _registered.TryAdd(id, tcs.Task);
        return new ShutdownHook(() =>
        {
            tcs.SetResult();
            _registered.Remove(id, out _);
        }, _shutdownSource.Task);
    }

    /// <inheritdoc />
    public Task ShutdownGracefully(CancellationToken cancellationToken)
    {
        _shutdownSource.SetResult();
        return Task.WhenAll(_registered.Values).WaitAsync(cancellationToken);
    }
    
    class ShutdownHook : IShutdownHook
    {
        readonly Action _onCompleted;

        public ShutdownHook(Action onCompleted, Task shuttingDown)
        {
            _onCompleted = onCompleted;
            ShuttingDown = shuttingDown;
        }

        public Task ShuttingDown { get; }

        public void MarkCompleted() => _onCompleted();
    }
}
