
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Protobuf;
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

    [HttpPost("commit")]
    public async Task<IActionResult> Commit(CommitEventsRequest request)
    {
        var result = await _eventStore.CommitEvents(request, HttpContext.RequestAborted).ConfigureAwait(false);
        if (result.Failure == default)
        {
            return Ok(CommitResponse.From(new CommittedEvents(result.Events.Select(_ => _.ToCommittedEvent()).ToList())));
        }
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new JsonResult(CommitResponse.From(result.Failure));
    }
    [HttpPost("commitForAggregate")]
    public async Task<IActionResult> CommitForAggregate(CommitForAggregateRequest request)
    {
        var result = await _eventStore.CommitAggregateEvents(request, HttpContext.RequestAborted).ConfigureAwait(false);
        if (result.Failure == default)
        {
            return Ok(CommitForAggregateResponse.From(result.Events.ToCommittedEvents()));
        }
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new JsonResult(CommitForAggregateResponse.From(result.Failure, request.AggregateEvents.EventSource, request.AggregateEvents.AggregateRoot));
    }
    [HttpPost("fetchForAggregate")]
    public async Task<IActionResult> FetchForAggregate(FetchForAggregateRequest request)
    {
        var result = await _eventStore.FetchAggregateEvents(request, HttpContext.RequestAborted).ConfigureAwait(false);
        if (result.Failure == default)
        {
            return Ok(FetchForAggregateResponse.From(result.Events.ToCommittedEvents()));
        }
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new JsonResult(FetchForAggregateResponse.From(result.Failure, request.EventSource, request.AggregateRoot));
    }
}
