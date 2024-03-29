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
/// Shutdown hooks allow the registrant to perform necessary cleanup actions asynchronously on shutdown before marking the hook complete,
/// allowing the shutdown to continue
/// </summary>
public interface IShutdownHook : IDisposable
{
    /// <summary>
    /// Gets a <see cref="Task"/> that completes when shutdown is triggered.
    /// </summary>
    Task ShuttingDown { get; }
    CancellationToken SystemStoppingToken { get; }

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
    /// It will prevent the application from shutting down until the hook is completed.
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
/// Defines a system that knows about stream processor lifecycle hooks.
/// This is a separate interface from <see cref="IApplicationLifecycleHooks"/> to allow for the stream processors to be shutdown before the application.
/// </summary>
public interface IStreamProcessorLifecycleHooks
{
    /// <summary>
    /// Register a shutdown hook.
    /// It will prevent the processor from shutting down until the hook is completed.
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
public class ApplicationLifecycleHooks: LifecycleHooks, IApplicationLifecycleHooks
{
}

/// <summary>
/// Represents an implementation of <see cref="IApplicationLifecycleHooks"/>.
/// </summary>
[Singleton]
public class StreamProcessorLifecycleHooks: LifecycleHooks, IStreamProcessorLifecycleHooks
{
}

// ReSharper disable once ClassNeverInstantiated.Global
public abstract class LifecycleHooks
{
    int _i = 0;

    readonly TaskCompletionSource _shutdownSource = new();
    readonly CancellationTokenSource _shutdownTokenSource = new();
    readonly ConcurrentDictionary<int, Task> _registered = new();

    /// <inheritdoc />
    public IShutdownHook RegisterShutdownHook()
    {
        _shutdownTokenSource.Token.ThrowIfCancellationRequested();
        var id = Interlocked.Increment(ref _i);
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _registered.TryAdd(id, tcs.Task);
        return new ShutdownHook(() =>
        {
            if (tcs.TrySetResult())
            {
                _registered.Remove(id, out _);
            }
        }, _shutdownSource.Task, _shutdownTokenSource.Token);
    }

    /// <inheritdoc />
    public Task ShutdownGracefully(CancellationToken cancellationToken)
    {
        try
        {
            _shutdownTokenSource.Cancel();
        }
        catch
        {
            // ignored
        }
        _shutdownSource.SetResult();
        return Task.WhenAll(_registered.Values).WaitAsync(cancellationToken);
    }

    class ShutdownHook : IShutdownHook
    {
        readonly Action _onCompleted;

        public ShutdownHook(Action onCompleted, Task shuttingDown, CancellationToken systemStoppingToken)
        {
            _onCompleted = onCompleted;
            SystemStoppingToken = systemStoppingToken;
            ShuttingDown = shuttingDown;
        }

        public Task ShuttingDown { get; }
        public CancellationToken SystemStoppingToken { get; }

        public void MarkCompleted() => _onCompleted();
        
        public void Dispose() => _onCompleted();
    }
}
