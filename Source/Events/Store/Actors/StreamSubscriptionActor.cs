// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class StreamSubscriptionActor : IActor
{
    /// <summary>
    /// If the subscription is further back than this, it will not store committed events
    /// to be streamed directly. Instead it will query for them when it catches up.
    /// </summary>
    const int StartBufferingWhenThisManyEventsBehind = 100;

    /// <summary>
    /// The maximum number of bytes to buffer in memory before falling back to catchup mode.
    /// </summary>
    const int MaxBufferedBytes = 10 * 1024 * 1024;

    readonly ScopeId _scope;
    readonly TenantId _tenantId;
    readonly ILogger<StreamSubscriptionActor> _logger;

    Func<Contracts.CommittedEvent, bool>? _shouldIncludeEvent;
    PID _target;
    Uuid _subscriptionId;
    string _subscriptionName;
    PID _catchupPid;

    ulong _nextPublishOffset;
    ulong _catchupTo;
    ulong _expectedNextCommitOffset;
    bool _receivedFirstCommit = false;


    bool _isCatchingUp = false;
    readonly EventBuffer _waitingEvents = new();

    int _publishRequestsInFlight = 0;
    int _catchUpRequestsInFlight = 0;
    HashSet<Guid> _artifactSet;

    long EventsBehind => (long)_catchupTo - (long)_nextPublishOffset - 1;

    public StreamSubscriptionActor(ScopeId scope, TenantId tenantId, ILogger<StreamSubscriptionActor> logger)
    {
        _scope = scope;
        _tenantId = tenantId;
        _logger = logger;
    }

    public async Task ReceiveAsync(IContext context)
    {
        try
        {
            await (context.Message switch
            {
                StartEventStoreSubscription request => OnStartEventStoreSubscription(request, context),
                CommittedEventsRequest request => OnCommittedEventsRequest(request, context),
                EventLogCatchupResponse response => OnCatchupResponse(response, context),
                SubscriptionEventsAck response => OnPublishAcknowledged(response, context),
                DeadLetterResponse deadLetter => OnDeadLetterResponse(deadLetter, context),
                _ => Task.CompletedTask
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in subscription actor");
            TerminateNonGracefully(context, e.Message);
        }
    }

    Task OnPublishAcknowledged(SubscriptionEventsAck _, IContext context)
    {
        _publishRequestsInFlight--;
        if (!_isCatchingUp)
        {
            FlushWaitingEvents(context);
            return Task.CompletedTask;
        }

        if (EventsBehind <= 0)
        {
            // Finished catching up. Publish any waiting events and go to streaming mode
            _isCatchingUp = false;
            FlushWaitingEvents(context);
            return Task.CompletedTask;
        }

        if (_waitingEvents.Overflowed)
        {
            if (_waitingEvents.Events.FromOffset >= _nextPublishOffset)
            {
                // The buffered events are next up, publish them
                FlushWaitingEvents(context);
                if (_catchUpRequestsInFlight == 0 && _publishRequestsInFlight <= 2)
                {
                    RequestMoreCatchupEvents(context, _nextPublishOffset, _catchupTo);
                }
            }
            else
            {
                // Catch up first
                RequestMoreCatchupEvents(context, _nextPublishOffset, _waitingEvents.Events!.FromOffset);
            }

            return Task.CompletedTask;
        }

        // Still catching up, request more events
        if (_catchUpRequestsInFlight == 0 && _publishRequestsInFlight <= 2)
        {
            RequestMoreCatchupEvents(context, _nextPublishOffset, _catchupTo);
        }

        return Task.CompletedTask;
    }

    Task OnDeadLetterResponse(DeadLetterResponse deadLetter, IContext context)
    {
        // This can be either the subscription client/target or the catchup actor
        if (_target.Equals(deadLetter.Target))
        {
            _logger.SubscriptionReturnedDeadLetter(_subscriptionName);
            context.Stop(context.Self);
        }
        else if (_catchupPid.Equals(deadLetter.Target))
        {
            _logger.LogError("Catchup actor died");
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
            RequestMoreCatchupEvents(context, _nextPublishOffset, _catchupTo);
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
        var events = _waitingEvents.Pop(_nextPublishOffset);
        if (events is not null)
        {
            Publish(events, context);
        }
    }

    Task OnStartEventStoreSubscription(StartEventStoreSubscription request, IContext context)
    {
        _subscriptionId = request.SubscriptionId;
        _subscriptionName = request.SubscriptionName;
        _target = PID.FromAddress(request.PidAddress, request.PidId);
        _shouldIncludeEvent = CreateFilter(request.EventTypeIds);
        _catchupPid = PID.FromAddress(context.System.Address, EventStoreCatchupActor.GetActorName(_tenantId));
        _nextPublishOffset = request.FromOffset;
        _catchupTo = request.CurrentHighWatermark;
        _expectedNextCommitOffset = _catchupTo;
        _artifactSet = request.EventTypeIds.Select(it => it.ToGuid()).ToHashSet();

        if (_nextPublishOffset < _catchupTo)
        {
            RequestMoreCatchupEvents(context, _nextPublishOffset, _catchupTo);
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
        context.Request(_catchupPid, new EventLogCatchupRequest(_scope, fromOffset, toOffset, _artifactSet));
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
        if (!_receivedFirstCommit)
        {
            _receivedFirstCommit = true;
            // If there is a race condition between the startup of the subscription actor and new commits, we need to
            // ensure that we don't miss any events. This is done by catching up to the current high watermark.
            if (request.FromOffset > _expectedNextCommitOffset)
            {
                _catchupTo = request.FromOffset;
                _expectedNextCommitOffset = request.FromOffset;
                // If it is not already working on it, start catching up
                if (!_isCatchingUp)
                {
                    RequestMoreCatchupEvents(context, _nextPublishOffset, _catchupTo);
                }
            }
        }

        if (_expectedNextCommitOffset != request.FromOffset)
        {
            _logger.LogError("Expected from offset {ExpectedFromOffset} but got {FromOffset}", _expectedNextCommitOffset, request.FromOffset);
            TerminateNonGracefully(context, $"Expected commit offset {_expectedNextCommitOffset} but got {request.FromOffset}");
        }

        _expectedNextCommitOffset = request.ToOffset + 1;

        if (_waitingEvents.Overflowed)
        {
            // No more space in the buffer, drop events until the current set is processed
            _catchupTo = request.ToOffset + 1;
            return Task.CompletedTask;
        }

        if (!_isCatchingUp)
        {
            var subscriptionEvent = ToSubscriptionEvent(request);
            if (_publishRequestsInFlight < 2)
            {
                // This is the normal case, publish events directly.
                Publish(subscriptionEvent, context);
                return Task.CompletedTask;
            }

            // If there are too many events in-flight, we might need to fall back to catch-up and drop streamed events
            if (!_waitingEvents.TryAdd(subscriptionEvent))
            {
                // Cache is full, go back to catchup mode
                _isCatchingUp = true;
                _catchupTo = request.ToOffset + 1;
            }

            return Task.CompletedTask;
        }

        // Catching up
        if (EventsBehind > StartBufferingWhenThisManyEventsBehind)
        {
            // Too far behind, catchup will be done in the background
            _catchupTo = request.ToOffset + 1;
            _waitingEvents.Reset();
            return Task.CompletedTask;
        }

        // Close enough, cache events for publishing when we are caught up
        var addedToBuffer = _waitingEvents.TryAdd(ToSubscriptionEvent(request));

        if (!addedToBuffer)
        {
            // Full buffer, so we need to make sure the subscription knows it needs to catch up again
            _catchupTo = request.ToOffset + 1;
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
        context.Stop(context.Self);
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


    sealed class EventBuffer
    {
        /// <summary>
        /// If the streaming buffer is full, drop new events until the current set is processed.
        /// The actor will then go into catchup mode.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Events))]
        public bool Overflowed { get; private set; }

        public SubscriptionEvents? Events { get; private set; }
        int _bufferedSize = 0;

        /// <summary>
        /// Tries to add the given events to the waiting events, if there is quota left.
        /// If the total size of the waiting events exceeds the quota, the waiting events are dropped, and false is returned.
        /// </summary>
        /// <param name="subscriptionEvents"></param>
        /// <returns></returns>
        public bool TryAdd(SubscriptionEvents subscriptionEvents)
        {
            var contentSize = subscriptionEvents.CalculateSize();
            if (_bufferedSize + contentSize > MaxBufferedBytes)
            {
                if (Events != null)
                {
                    Overflowed = true;
                }

                return false;
            }

            _bufferedSize += contentSize;
            Events = Events?.Merge(subscriptionEvents) ?? subscriptionEvents;

            return true;
        }

        /// <summary>
        /// Get events that are ready to be published, and remove them from the cache.
        /// </summary>
        /// <param name="fromOffset"></param>
        /// <returns></returns>
        public SubscriptionEvents? Pop(ulong fromOffset)
        {
            if (Events is null)
            {
                return null;
            }

            var events = Events.CutoffEarlierThan(fromOffset);
            Reset();
            return events;
        }

        public void Reset()
        {
            Overflowed = false;
            Events = null;
            _bufferedSize = 0;
        }
    }
}
