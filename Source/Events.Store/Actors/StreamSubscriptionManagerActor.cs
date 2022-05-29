// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Persistence;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

[Scoped]
public class StreamSubscriptionManagerActor : IActor
{
    readonly Props _streamSubscriptionProps;
    readonly Dictionary<Guid, PID> _activeSubscriptions = new();
    readonly ScopeId _scope;
    EventLogSequenceNumber _nextSequenceNumber;
    readonly ILogger<StreamSubscriptionManagerActor> _logger;

    public StreamSubscriptionManagerActor(
        ScopeId scope,
        EventLogSequenceNumber initialNextSequenceNumber,
        TenantId tenantId,
        ICreateProps propsFactory,
        ILogger<StreamSubscriptionManagerActor> logger)
    {
        _scope = scope;
        _nextSequenceNumber = initialNextSequenceNumber;
        _logger = logger;
        _streamSubscriptionProps = propsFactory.PropsFor<StreamSubscriptionActor>(scope, tenantId);
    }

    public Task ReceiveAsync(IContext context)
    {
        return context.Message switch
        {
            EventStoreSubscriptionRequest request => OnEventStoreSubscriptionRequest(request, context),
            CancelEventStoreSubscription request => OnCancelEventStoreSubscription(request, context),
            Commit request => OnCommit(request, context),
            _ => Task.CompletedTask
        };
    }

    Task OnCancelEventStoreSubscription(CancelEventStoreSubscription request, IContext context)
    {
        var id = request.SubscriptionId.ToGuid();
        if (_activeSubscriptions.TryGetValue(id, out var pid))
        {
            context.Stop(pid);
            _activeSubscriptions.Remove(id);
        }

        context.Respond(new CancelEventStoreSubscriptionAck
        {
            SubscriptionId = request.SubscriptionId
        });

        return Task.CompletedTask;
    }

    Task OnCommit(Commit commit, IContext context)
    {
        var request = new CommittedEventsRequest
        {
            Events = { commit.AllEvents.Select(it => it.ToProtobuf()) },
            FromOffset = commit.FirstSequenceNumber,
            ToOffset = commit.LastSequenceNumber
        };
        foreach (var kv in _activeSubscriptions)
        {
            context.Send(kv.Value, request);
        }

        _nextSequenceNumber = request.ToOffset + 1;
        return Task.CompletedTask;
    }

    Task OnEventStoreSubscriptionRequest(EventStoreSubscriptionRequest request, IContext context)
    {
        var id = request.SubscriptionId.ToGuid();
        if (_activeSubscriptions.ContainsKey(id))
        {
            context.Respond(new EventStoreSubscriptionAck
            {
                SubscriptionId = request.SubscriptionId,
                Ok = false
            });
            return Task.CompletedTask;
        }

        var pid = context.Spawn(_streamSubscriptionProps);
        _activeSubscriptions[id] = pid;
        context.Send(pid, new StartEventStoreSubscription
        {
            EventTypeIds = { request.EventTypeIds },
            FromOffset = request.FromOffset,
            PidId = request.PidId,
            PidAddress = request.PidAddress,
            SubscriptionId = request.SubscriptionId,
            ScopeId = request.ScopeId,
            CurrentHighWatermark = _nextSequenceNumber
        });
        context.Respond(new EventStoreSubscriptionAck
        {
            SubscriptionId = request.SubscriptionId,
            Ok = true
        });
        return Task.CompletedTask;
    }
}
