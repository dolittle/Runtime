// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Events.Store.Actors;

record EventLogCatchupRequest(EventLogSequenceNumber From, int MaxCount);

public class EventStoreCatchupActor : IActor
{
    const int BatchSize = 1000;

    readonly IFetchCommittedEvents _eventsFetcher;
    readonly ILogger<EventStoreCatchupActor> _logger;

    public EventStoreCatchupActor(IFetchCommittedEvents eventsFetcher, ILogger<EventStoreCatchupActor> logger)
    {
        _eventsFetcher = eventsFetcher;
        _logger = logger;
    }


    public Task ReceiveAsync(IContext context)
    {
        return context.Message switch
        {
            EventLogCatchupRequest request => OnEventLogCatchupRequest(request, context),
            _ => Task.CompletedTask
        };
    }

    Task OnEventLogCatchupRequest(EventLogCatchupRequest request, IContext context)
    {
        var maxEvents = Math.Min(request.MaxCount, BatchSize);
        if (maxEvents < 1)
        {
            context.Respond(new CommittedEventsRequest
            {
                FromOffset = request.From,
                ToOffset = request.From
            });
            return Task.CompletedTask;
        }


        context.ReenterAfter(_eventsFetcher.FetchCommittedEvents(request.From, maxEvents, context.CancellationToken), task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                var committedEvents = task.Result;
                var fromOffset = request.From;
                var toOffset = committedEvents.Count > 0 ? committedEvents[^1].EventLogSequenceNumber : fromOffset;
                context.Respond(new CommittedEventsRequest
                    {
                        Events = { committedEvents.ToProtobuf() },
                        FromOffset = fromOffset,
                        ToOffset = toOffset,
                    }
                );
            }
            else
            {
                var aggregateException = task.Exception;
                if (aggregateException is not null)
                {
                    _logger.ErrorFetchingCatchupEvents(aggregateException);
                }
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    return OnEventLogCatchupRequest(request, context); // Retry
                }
            }

            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }

    public static string GetActorName(TenantId tenant)
    {
        return $"es-catchup-{tenant.Value:N}";
    }
}
