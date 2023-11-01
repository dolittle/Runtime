// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    readonly TenantId _tenantId;
    readonly IFetchCommittedEvents _committedEvents;
    readonly ILogger<StreamSubscriptionManagerActor> _logger;

    EventLogSequenceNumber _nextSequenceNumber;
    public StreamSubscriptionManagerActor(
        ScopeId scope,
        TenantId tenantId,
        IFetchCommittedEvents committedEvents,
        ICreateProps propsFactory,
        ILogger<StreamSubscriptionManagerActor> logger)
    {
        _scope = scope;
        _tenantId = tenantId;
        _committedEvents = committedEvents;
        _logger = logger;
        _streamSubscriptionProps = propsFactory.PropsFor<StreamSubscriptionActor>(scope);
    }

    public Task ReceiveAsync(IContext context)
    {
            return context.Message switch
            {
                Started => OnStarted(context.CancellationToken),
                EventStoreSubscriptionRequest request => OnEventStoreSubscriptionRequest(request, context),
                CancelEventStoreSubscription request => OnCancelEventStoreSubscription(request, context),
                Commit request => OnCommit(request, context),
                _ => Task.CompletedTask
            };
    }

    async Task OnStarted(CancellationToken cancellationToken)
    {
        _nextSequenceNumber = await _committedEvents.FetchNextSequenceNumber(_scope, cancellationToken).ConfigureAwait(false);
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
                ScopeId = request.ScopeId,
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
            CurrentHighWatermark = _nextSequenceNumber,
            SubscriptionName = request.SubscriptionName
        });
        
        context.Respond(new EventStoreSubscriptionAck
        {
            SubscriptionId = request.SubscriptionId,
            Ok = true
        });
        return Task.CompletedTask;
    }
}
