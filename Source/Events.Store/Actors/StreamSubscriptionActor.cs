// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Artifacts.Contracts;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

public class StreamSubscriptionActor : IActor
{
    readonly TenantId _tenantId;
    readonly ILogger<StreamSubscriptionActor> _logger;

    Func<Contracts.CommittedEvent, bool> _shouldIncludeEvent;
    PID _target;
    Uuid _subscriptionId;

    public StreamSubscriptionActor(TenantId tenantId, ILogger<StreamSubscriptionActor> logger)
    {
        _tenantId = tenantId;
        _logger = logger;
    }

    public Task ReceiveAsync(IContext context)
    {
        return context.Message switch
        {
            StartEventStoreSubscription request => OnStartEventStoreSubscription(request, context),
            CommittedEventsRequest request => OnCommittedEventsRequest(request, context),
            _ => Task.CompletedTask
        };
    }

    async Task OnStartEventStoreSubscription(StartEventStoreSubscription request, IContext context)
    {
        _subscriptionId = request.SubscriptionId;
        _target = PID.FromAddress(request.PidAddress, request.PidId);
        _shouldIncludeEvent = CreateFilter(request.EventTypes);
        var fromOffset = request.FromOffset;
        var toOffset = request.CurrentHighWatermark;

        var catchupPid = PID.FromAddress(context.System.Address, EventStoreCatchupActor.GetActorName(_tenantId));
        
        while (fromOffset < toOffset && !context.CancellationToken.IsCancellationRequested)
        {
            try
            {
                var response = await context.RequestAsync<CommittedEventsRequest>(catchupPid,
                    new EventLogCatchupRequest(fromOffset, (int)(toOffset - fromOffset)), context.CancellationToken);
                await OnCommittedEventsRequest(response, context);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                
                _logger.LogError(e, "failed to catchup events. retrying");
            }
        }
    }

    static Func<global::Dolittle.Runtime.Events.Contracts.CommittedEvent,bool> CreateFilter(IEnumerable<Artifact> eventTypes)
    {
        var eventSet = eventTypes.Select(artifact => artifact.Id).ToHashSet();

        return committedEvent => eventSet.Contains(committedEvent.EventType.Id);
    }

    async Task OnCommittedEventsRequest(CommittedEventsRequest request, IContext context)
    {
        var subscriptionEvent = ToSubscriptionEvent(request);
        while (!context.CancellationToken.IsCancellationRequested)
        {
            try
            {
                var response = await context.RequestAsync<SubscriptionEventsAck>(_target, subscriptionEvent, context.CancellationToken);
                if (response != null)
                {
                    return;
                }
            }
            catch (DeadLetterException)
            {
                _logger.LogError("subscription target returned deadletter");
                context.Stop(context.Self);
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e,"failed to publish subscription events");
            }
        }
    }

    SubscriptionEvents ToSubscriptionEvent(CommittedEventsRequest request) =>
        new()
        {
            SubscriptionId = _subscriptionId,
            FromOffset = request.FromOffset,
            ToOffset = request.ToOffset,
            Events = { request.Events.Where(_shouldIncludeEvent) }
        };
}
