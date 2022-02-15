
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

[Route("api/events")]
[ApiController]
public class EventStoreController : ControllerBase
{
    readonly IEventStoreService _eventStoreService;

    public EventStoreController(IEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }

    [HttpPost("commit")]
    public async Task<IActionResult> Commit(CommitRequest request)
    {
        var commitResult = await _eventStoreService.TryCommit(
            new UncommittedEvents(new ReadOnlyCollection<UncommittedEvent>(request.Events.Select(_ => _.ToUncommittedEvent()).ToList())),
            request.CallContext.ExecutionContext.ToExecutionContext(),
            System.Threading.CancellationToken.None).ConfigureAwait(false);
        if (commitResult.Success)
        {
            return Ok(CommitResponse.From(commitResult.Result));
        }
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new JsonResult(CommitResponse.From(commitResult.Exception.ToFailure()));
    }
    [HttpPost("commitForAggregate")]
    public async Task<IActionResult> CommitForAggregate(CommitForAggregateRequest request)
    {
        var commitResult = await _eventStoreService.TryCommitForAggregate(
            request.AggregateEvents.ToUncommittedAggregateEvents(),
            request.CallContext.ExecutionContext.ToExecutionContext(),
            System.Threading.CancellationToken.None).ConfigureAwait(false);
        if (commitResult.Success)
        {
            return Ok(CommitForAggregateResponse.From(commitResult.Result));
        }
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new JsonResult(CommitForAggregateResponse.From(commitResult.Exception.ToFailure(), request.AggregateEvents.EventSource, request.AggregateEvents.AggregateRoot));
    }
    [HttpPost("fetchForAggregate")]
    public async Task<IActionResult> FetchForAggregate(FetchForAggregateRequest request)
    {
        var fetchResult = await _eventStoreService.TryFetchForAggregate(
            request.AggregateRoot,
            request.EventSource,
            request.CallContext.ExecutionContext.ToExecutionContext(),
            System.Threading.CancellationToken.None).ConfigureAwait(false);
        if (fetchResult.Success)
        {
            return Ok(FetchForAggregateResponse.From(fetchResult.Result));
        }
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new JsonResult(FetchForAggregateResponse.From(fetchResult.Exception.ToFailure(), request.EventSource, request.AggregateRoot));
    }
}