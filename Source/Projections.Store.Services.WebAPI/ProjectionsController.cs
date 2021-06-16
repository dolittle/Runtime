
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dolittle.Runtime.Projections.Store.Services.WebAPI
{
    [Route("api/projections")]
    [ApiController]
    public class EventStoreController : ControllerBase
    {
        readonly IProjectionsService _projectionsService;

        public EventStoreController(IProjectionsService projectionsService)
        {
            _projectionsService = projectionsService;
        }

        [HttpPost("getOne")]
        public async Task<IActionResult> GetOne(GetOneRequest request)
        {
            var getOneResult = await _projectionsService.TryGetOne(
                request.Projection,
                request.Scope,
                request.Key,
                request.Context.ExecutionContext.ToExecutionContext(),
                System.Threading.CancellationToken.None).ConfigureAwait(false);
            if (getOneResult.Success) return Ok(GetOneResponse.From(getOneResult.Result));
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new JsonResult(GetOneResponse.From(getOneResult.Exception.ToFailure()));
        }
        [HttpPost("getAll")]
        public async Task<IActionResult> GetAll(GetAllRequest request)
        {
            var getAllResult = await _projectionsService.TryGetAll(
                request.Projection,
                request.Scope,
                request.Context.ExecutionContext.ToExecutionContext(),
                System.Threading.CancellationToken.None).ConfigureAwait(false);
            if (getAllResult.Success) return Ok(GetAllResponse.From(getAllResult.Result));
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new JsonResult(GetAllResponse.From(getAllResult.Exception.ToFailure()));
        }
    }
}