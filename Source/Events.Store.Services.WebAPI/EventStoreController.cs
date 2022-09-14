// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary.AsyncEnumerators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

[Route("api/events")]
[ApiController]
public class EventStoreController : ControllerBase
{
    readonly IEventStore _eventStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreController"/> class.
    /// </summary>
    /// <param name="eventStore">The event store to use.</param>
    public EventStoreController(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    /// <summary>
    /// Commits events.
    /// </summary>
    /// <param name="request">The commit request.</param>
    /// <returns>The commit response.</returns>
    [HttpPost("commit")]
    public async Task<CommitResponse> Commit(CommitEventsRequest request)
    {
        var result = await _eventStore.CommitEvents(request, HttpContext.RequestAborted).ConfigureAwait(false);
        if (result.Failure is not null)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }
    
    /// <summary>
    /// Commits events for an aggregate root.
    /// </summary>
    /// <param name="request">The commit request.</param>
    /// <returns>The commit response.</returns>
    [HttpPost("commitForAggregate")]
    public async Task<CommitForAggregateResponse> CommitForAggregate(CommitForAggregateRequest request)
    {
        var result = await _eventStore.CommitAggregateEvents(request, HttpContext.RequestAborted).ConfigureAwait(false);
        if (result.Failure is not null)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }
    
    /// <summary>
    /// Fetches committed events for an aggregate root instance.
    /// </summary>
    /// <param name="request">The fetch request.</param>
    /// <returns>The fetch response.</returns>
    [HttpPost("fetchForAggregate")]
    public async Task<FetchForAggregateResponse> FetchForAggregate(FetchForAggregateRequest request)
    {
        var result = await _eventStore
            .FetchAggregateEvents(request, HttpContext.RequestAborted)
            .BatchReduceMessagesOfSize(
                (first, next) =>
                {
                    first.Events.Events.AddRange(next.Events.Events);
                    first.Failure ??= next.Failure;

                    return first;
                },
                uint.MaxValue,
                HttpContext.RequestAborted)
            .SingleAsync(HttpContext.RequestAborted)
            .ConfigureAwait(false);
        
        if (result.Failure is not null)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }
}
