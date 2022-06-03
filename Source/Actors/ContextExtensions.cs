// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Proto;

namespace Dolittle.Runtime.Actors;

/// <summary>
/// Implements extension methods for <see cref="IContext"/>.
/// </summary>
public static class ContextExtensions
{
    public static bool TrySpawn(this IContext context, Props props, [NotNullWhen(true)] out PID pid, [NotNullWhen(false)] out Exception error)
        => TrySpawn(() => context.Spawn(props), out pid, out error);
    
    public static bool TrySpawnNamed(this IContext context, Props props, string name, [NotNullWhen(true)] out PID pid, [NotNullWhen(false)] out Exception error)
        => TrySpawn(() => context.SpawnNamed(props, name), out pid, out error);
    
    public static bool TrySpawnNamed(this IContext context, Props props, string name, Action<IContext> callback, [NotNullWhen(true)] out PID pid, [NotNullWhen(false)] out Exception error)
        => TrySpawn(() => context.SpawnNamed(props, name, callback), out pid, out error);
    
    public static bool TrySpawnPrefix(this IContext context, Props props, string prefix, [NotNullWhen(true)] out PID pid, [NotNullWhen(false)] out Exception error)
        => TrySpawn(() => context.SpawnPrefix(props, prefix), out pid, out error);
    
    public static bool TrySpawnPrefix(this IContext context, Props props, string prefix, Action<IContext> callback, [NotNullWhen(true)] out PID pid, [NotNullWhen(false)] out Exception error)
        => TrySpawn(() => context.SpawnPrefix(props, prefix, callback), out pid, out error);
    

    static bool TrySpawn(Func<PID> spawn, [NotNullWhen(true)] out PID pid, [NotNullWhen(false)] out Exception error)
    {
        pid = default;
        error = default;
        try
        {
            pid = spawn();
            return true;
        }
        catch (Exception e)
        {
            error = e;
            return false;
        }
    }
}
