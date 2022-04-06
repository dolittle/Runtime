// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto;

namespace Dolittle.Runtime.Actors.Hosting;

record RegisterShutdownHook(PID Who);

record UnRegisterShutdownHook(PID Who);

record Ack;

record ShuttingDown;

record ShutdownAck;

public class LifecycleActor : IActor
{
    readonly HashSet<PID> _registeredTargets = new();

    public async Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case RegisterShutdownHook register:
                OnRegister(register, context);
                break;
            case UnRegisterShutdownHook unRegister:
                OnUnRegister(unRegister, context);
                break;
            case ShuttingDown msg:
                await OnShutdown(msg, context);
                break;
        }
    }

    async Task OnShutdown(ShuttingDown shuttingDown, IContext context)
    {
        if (_registeredTargets.Any())
        {
            await Task.WhenAll(_registeredTargets.Select(target =>
                context.RequestAsync<ShutdownAck>(target, shuttingDown, context.CancellationToken)));
        }

        context.Respond(new ShutdownAck());
    }

    void OnRegister(RegisterShutdownHook register, IContext context)
    {
        _registeredTargets.Add(register.Who);
        context.Respond(new Ack());
    }

    void OnUnRegister(UnRegisterShutdownHook unRegister, IContext context)
    {
        _registeredTargets.Add(unRegister.Who);
        context.Respond(new Ack());
    }
}
