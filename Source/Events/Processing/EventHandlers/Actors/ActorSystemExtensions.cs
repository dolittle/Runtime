// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Proto;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.Actors;

static class ActorSystemExtensions
{
    // Wait for the actor represented by the PID to be terminated
    public static Task WatchAsync(this ActorSystem system, PID pid)
    {
        var tcs = new TaskCompletionSource();
        system.Root.Spawn(Props.FromFunc(ctx =>
        {
            switch (ctx.Message)
            {
                case Started:
                    ctx.Watch(pid);
                    return Task.CompletedTask;

                case Terminated:
                    tcs.SetResult();
                    ctx.Stop(ctx.Self);
                    return Task.CompletedTask;

                default:
                    return Task.CompletedTask;
            }
        }));
        return tcs.Task;
    }
}
