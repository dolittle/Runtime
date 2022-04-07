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
    Task ShuttingDown { get; }
    void MarkCompleted();
}

public interface IApplicationLifecycleHooks
{
    IShutdownHook RegisterShutdownHook();
    Task ShutdownGracefully(CancellationToken cancellationToken);
}

[Singleton]
// ReSharper disable once ClassNeverInstantiated.Global
public class ApplicationLifecycleHooks : IApplicationLifecycleHooks
{
    int _i = 0;

    readonly TaskCompletionSource _shutdownSource = new();
    readonly ConcurrentDictionary<int, Task> _registered = new();


    public IShutdownHook RegisterShutdownHook()
    {
        var id = Interlocked.Increment(ref _i);
        var tcs = new TaskCompletionSource();
        _registered.TryAdd(id, tcs.Task);
        return new ShutdownHook(() =>
        {
            tcs.SetResult();
            _registered.Remove(id, out _);
        }, _shutdownSource.Task);
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

    public Task ShutdownGracefully(CancellationToken cancellationToken)
    {
        _shutdownSource.SetResult();
        
        return Task.WhenAll(_registered.Values).WaitAsync(cancellationToken);
    }
}
