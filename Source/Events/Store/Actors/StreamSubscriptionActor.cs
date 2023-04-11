// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

// ReSharper disable once ClassNeverInstantiated.Global
public class StreamSubscriptionActor : IActor
{
    const int MinWaitMs = 100;
    const int MaxJitterMs = 400;
    readonly ScopeId _scope;
    readonly TenantId _tenantId;
    readonly ILogger<StreamSubscriptionActor> _logger;

    Func<Contracts.CommittedEvent, bool>? _shouldIncludeEvent;
    PID _target;
    Uuid _subscriptionId;
    string _subscriptionName;

    public StreamSubscriptionActor(ScopeId scope, TenantId tenantId, ILogger<StreamSubscriptionActor> logger)
    {
        _scope = scope;
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
        _subscriptionName = request.SubscriptionName;
        _target = PID.FromAddress(request.PidAddress, request.PidId);
        _shouldIncludeEvent = CreateFilter(request.EventTypeIds);
        var fromOffset = request.FromOffset;
        var toOffset = request.CurrentHighWatermark;
        var catchupPid = PID.FromAddress(context.System.Address, EventStoreCatchupActor.GetActorName(_tenantId));
        while (fromOffset < toOffset && !context.CancellationToken.IsCancellationRequested)
        {
            try
            {
                var response = await context.RequestAsync<EventLogCatchupResponse>(catchupPid,
                    new EventLogCatchupRequest(request.ScopeId.ToGuid(), fromOffset, (int)(toOffset + 1 - fromOffset)), context.CancellationToken);
                await Publish(ToSubscriptionEvent(response), context);
                fromOffset = response.ToOffset + 1;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                _logger.ErrorFetchingCatchupEvents(e);
                await Task.Delay(MinWaitMs + new Random().Next(MaxJitterMs));
            }
        }
    }

    static Func<global::Dolittle.Runtime.Events.Contracts.CommittedEvent, bool> CreateFilter(IEnumerable<Uuid> eventTypes)
    {
        var eventSet = eventTypes.Select(artifactId => artifactId).ToHashSet();
        if (!eventSet.Any())
        {
            // Empty set, consume all events
            return _ => true;
        }

        return committedEvent => eventSet.Contains(committedEvent.EventType.Id);
    }

    Task OnCommittedEventsRequest(CommittedEventsRequest request, IContext context)
        => Publish(ToSubscriptionEvent(request), context);

    async Task Publish(SubscriptionEvents subscriptionEvent, IContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            try
            {
                var response = await context.RequestAsync<SubscriptionEventsAck>(_target, subscriptionEvent, TimeSpan.FromSeconds(10));
                if (response != null)
                {
                    return;
                }
            }
            catch (DeadLetterException e)
            {
                _logger.SubscriptionReturnedDeadLetter(e);
                // ReSharper disable once MethodHasAsyncOverload
                context.Stop(context.Self);
                return;
            }
            catch (Exception e)
            {
                _logger.ErrorPublishingSubscribedEvents(e, _subscriptionName);
            }
        }
    }

    SubscriptionEvents ToSubscriptionEvent(CommittedEventsRequest request) =>
        new()
        {
            SubscriptionId = _subscriptionId,
            FromOffset = request.FromOffset,
            ToOffset = request.ToOffset,
            Events = { request.Events.Where(_shouldIncludeEvent!) }
        };

    SubscriptionEvents ToSubscriptionEvent(EventLogCatchupResponse response) =>
        new()
        {
            SubscriptionId = _subscriptionId,
            FromOffset = response.FromOffset,
            ToOffset = response.ToOffset,
            Events = { response.Events.Where(_shouldIncludeEvent!) }
        };
}
