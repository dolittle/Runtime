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
    /// <summary>
    /// If the subscription is further back than this, it will not store committed events
    /// to be streamed directly. Instead it will query for them when it catches up.
    /// </summary>
    const int MaxWaitingEvents = 1000;

    readonly ScopeId _scope;
    readonly TenantId _tenantId;
    readonly ILogger<StreamSubscriptionActor> _logger;

    Func<Contracts.CommittedEvent, bool>? _shouldIncludeEvent;
    PID _target;
    Uuid _subscriptionId;
    string _subscriptionName;
    PID _catchupPid;

    ulong _nextPublishOffset;
    ulong _catchupUpTo;
    ulong _expectedNextCommitOffset;
    bool _isCatchingUp;
    bool _receivedFirstCommit = false;

    SubscriptionEvents? _waitingEvents;
    int _publishRequestsInFlight = 0;
    int _catchUpRequestsInFlight = 0;
    HashSet<Guid> _artifactSet;

    long EventsBehind => (long)_catchupUpTo - (long)_nextPublishOffset - 1;

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
            EventLogCatchupResponse response => OnCatchupResponse(response, context),
            SubscriptionEventsAck response => OnPublishAcknowledged(response, context),
            DeadLetterResponse deadLetter => OnDeadLetterResponse(deadLetter, context),
            _ => Task.CompletedTask
        };
    }

    Task OnPublishAcknowledged(SubscriptionEventsAck response, IContext context)
    {
        _publishRequestsInFlight--;
        if (!_isCatchingUp)
        {
            // Only needs to request more events if in the process of catching up
            return Task.CompletedTask;
        }

        if (EventsBehind <= 0)
        {
            _isCatchingUp = false;
            FlushWaitingEvents(context);
            return Task.CompletedTask;
        }

        // Still catching up, request more events
        if (_catchUpRequestsInFlight == 0 && _publishRequestsInFlight <= 2)
        {
            RequestMoreCatchupEvents(context, _nextPublishOffset, _catchupUpTo);
        }
        return Task.CompletedTask;

    }

    Task OnDeadLetterResponse(DeadLetterResponse deadLetter, IContext context)
    {
        // This can be either the subscription client/target or the catchup actor
        if (_target.Equals(deadLetter.Target))
        {
            _logger.SubscriptionReturnedDeadLetter(_subscriptionName);
            // ReSharper disable once MethodHasAsyncOverload
            context.Stop(context.Self);
        }
        else if (_catchupPid.Equals(deadLetter.Target))
        {
            TerminateNonGracefully(context, "Catchup actor died");
        }

        return Task.CompletedTask;
    }

    Task OnCatchupResponse(EventLogCatchupResponse response, IContext context)
    {
        _catchUpRequestsInFlight--;
        if (response.FromOffset.Value != _nextPublishOffset)
        {
            _logger.LogError("Received catchup response with unexpected from offset {FromOffset} (expected {ExpectedFromOffset})",
                response.FromOffset, _nextPublishOffset);
            TerminateNonGracefully(context, $"Received catchup response with unexpected from offset {response.FromOffset} (expected {_nextPublishOffset})");
            return Task.CompletedTask;
        }
        
        Publish(ToSubscriptionEvent(response), context);
        if (EventsBehind > 0)
        {
            RequestMoreCatchupEvents(context, _nextPublishOffset, _catchupUpTo);
        }
        else
        {
            _isCatchingUp = false;
            FlushWaitingEvents(context);
        }
        return Task.CompletedTask;
    }


    void FlushWaitingEvents(IContext context)
    {
        if (_waitingEvents is null)
        {
            return;
        }

        var events = _waitingEvents.CutoffEarlierThan(_nextPublishOffset);
        if (events is null)
        {
            _waitingEvents = null;
            return;
        }

        Publish(events, context);
        _waitingEvents = null;
    }

    Task OnStartEventStoreSubscription(StartEventStoreSubscription request, IContext context)
    {
        _subscriptionId = request.SubscriptionId;
        _subscriptionName = request.SubscriptionName;
        _target = PID.FromAddress(request.PidAddress, request.PidId);
        _shouldIncludeEvent = CreateFilter(request.EventTypeIds);
        _catchupPid = PID.FromAddress(context.System.Address, EventStoreCatchupActor.GetActorName(_tenantId));
        _nextPublishOffset = request.FromOffset;
        _catchupUpTo = request.CurrentHighWatermark;
        _expectedNextCommitOffset = _catchupUpTo;
        _artifactSet = request.EventTypeIds.Select(it => it.ToGuid()).ToHashSet();

        if (_nextPublishOffset < _catchupUpTo)
        {
            RequestMoreCatchupEvents(context, _nextPublishOffset, _catchupUpTo);
        }
        else
        {
            _isCatchingUp = false;
        }


        return Task.CompletedTask;
    }

    /// <summary>
    /// Get old events that were missed while the subscription was not active.
    /// Done in the background, the actor will receive new committed events to know the current high watermark.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="fromOffset"></param>
    /// <param name="toOffset"></param>
    void RequestMoreCatchupEvents(IContext context, ulong fromOffset, ulong toOffset)
    {
        _isCatchingUp = true;
        _catchUpRequestsInFlight++;
        context.Request(_catchupPid, new EventLogCatchupRequest(_scope, fromOffset, _catchupUpTo, _artifactSet));
    }

    static Func<global::Dolittle.Runtime.Events.Contracts.CommittedEvent, bool> CreateFilter(IEnumerable<Uuid> eventTypes)
    {
        var eventSet = eventTypes.ToHashSet();
        if (!eventSet.Any())
        {
            // Empty set, consume all events
            return _ => true;
        }

        return committedEvent => eventSet.Contains(committedEvent.EventType.Id);
    }

    Task OnCommittedEventsRequest(CommittedEventsRequest request, IContext context)
    {
        // Ensure that we process all
        if (!_receivedFirstCommit)
        {
            _receivedFirstCommit = true;
            // If there is a race condition between the startup of the subscription actor and new commits, we need to
            // ensure that we don't miss any events. This is done by catching up to the current high watermark.
            if (request.FromOffset > _expectedNextCommitOffset)
            {
                _catchupUpTo = request.FromOffset;
                _expectedNextCommitOffset = request.FromOffset;
                // If it is not already working on it, start catching up
                if (!_isCatchingUp)
                {
                    RequestMoreCatchupEvents(context, _nextPublishOffset, _catchupUpTo);
                }
            }
        }

        if (_expectedNextCommitOffset != request.FromOffset)
        {
            _logger.LogError("Expected from offset {ExpectedFromOffset} but got {FromOffset}", _expectedNextCommitOffset, request.FromOffset);
            TerminateNonGracefully(context, $"Expected commit offset {_expectedNextCommitOffset} but got {request.FromOffset}");
        }

        _expectedNextCommitOffset = request.ToOffset + 1;

        if (!_isCatchingUp)
        {
            Publish(ToSubscriptionEvent(request), context);
        }

        if (EventsBehind > MaxWaitingEvents)
        {
            // Too far behind, catchup will be done in the background
            _catchupUpTo = request.ToOffset;
            _waitingEvents = null;
            return Task.CompletedTask;
        }

        // Close to being caught up, publish events when catch-up is completed
        var subscriptionEvents = ToSubscriptionEvent(request);
        if (_waitingEvents is not null)
        {
            _waitingEvents.Merge(subscriptionEvents);
        }
        else
        {
            _waitingEvents = subscriptionEvents;
        }

        return Task.CompletedTask;
    }

    void Publish(SubscriptionEvents subscriptionEvent, IContext context)
    {
        // Should never happen..
        if (subscriptionEvent.FromOffset != _nextPublishOffset)
        {
            _logger.LogError("Expected from offset {ExpectedFromOffset} but got {FromOffset}", _nextPublishOffset, subscriptionEvent.FromOffset);
            TerminateNonGracefully(context, $"Expected from offset {_nextPublishOffset} but got {subscriptionEvent.FromOffset}");
        }

        _nextPublishOffset = subscriptionEvent.ToOffset + 1;
        context.Request(_target, subscriptionEvent);
        _publishRequestsInFlight++;
    }

    void TerminateNonGracefully(IContext context, string message)
    {
        context.Send(_target, new SubscriptionWasReset
        {
            SubscriptionId = _subscriptionId,
            Reason = message
        });
        throw new ArgumentException(message);
    }

    SubscriptionEvents ToSubscriptionEvent(CommittedEventsRequest request)
    {
        return new()
        {
            SubscriptionId = _subscriptionId,
            FromOffset = request.FromOffset,
            ToOffset = request.ToOffset,
            Events = { request.Events.Where(_shouldIncludeEvent!) }
        };
    }

    SubscriptionEvents ToSubscriptionEvent(EventLogCatchupResponse response) =>
        new()
        {
            SubscriptionId = _subscriptionId,
            FromOffset = response.FromOffset,
            ToOffset = response.ToOffset,
            Events = { response.Events.Where(_shouldIncludeEvent!) }
        };
}
