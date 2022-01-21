// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Dolittle.Runtime.Projections.Contracts.Projections;

namespace Dolittle.Runtime.Projections.Store.Services.Grpc;

/// <summary>
/// Represents the implementation of.
/// </summary>
public class ProjectionsGrpcService : ProjectionsBase
{
    const uint MaxBatchMessageSize = 2097152; // 2 MB
    readonly IProjectionsService _projectionsService;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsGrpcService"/> class.
    /// </summary>
    /// <param name="projectionsService"><see cref="IProjectionsService"/>.</param>
    /// <param name="logger">The logger to use.</param>
    public ProjectionsGrpcService(
        IProjectionsService projectionsService,
        ILogger logger
    )
    {
        _projectionsService = projectionsService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task<GetOneResponse> GetOne(GetOneRequest request, ServerCallContext context)
    {
        var response = new GetOneResponse();
        var getOneResult = await _projectionsService.TryGetOne(
            request.ProjectionId.ToGuid(),
            request.ScopeId.ToGuid(),
            request.Key,
            request.CallContext.ExecutionContext.ToExecutionContext(),
            context.CancellationToken).ConfigureAwait(false);

        if (getOneResult.Success)
        {
            response.State = getOneResult.Result.ToProtobuf();
            Log.SendingGetOneResult(_logger, request.Key, request.ProjectionId, request.ScopeId, getOneResult.Result.Type);
        }
        else
        {
            response.Failure = getOneResult.Exception.ToFailure();
            Log.SendingGetOneFailed(_logger, request.Key, request.ProjectionId, request.ScopeId, getOneResult.Exception);
        }

        return response;
    }

    /// <inheritdoc/>
    public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var getAllResult = await _projectionsService.TryGetAll(
            request.ProjectionId.ToGuid(),
            request.ScopeId.ToGuid(),
            request.CallContext.ExecutionContext.ToExecutionContext(),
            context.CancellationToken).ConfigureAwait(false);

        if (getAllResult.Success)
        {
            response.States.AddRange(getAllResult.Result.ToProtobuf());
            Log.SendingGetAllResult(_logger, request.ProjectionId, request.ScopeId, response.States.Count);
        }
        else
        {
            response.Failure = getAllResult.Exception.ToFailure();
            Log.SendingGetAllFailed(_logger, request.ProjectionId, request.ScopeId, getAllResult.Exception);
        }

        return response;
    }

    /// <inheritdoc />
    public override async Task GetAllInBatches(GetAllRequest request, IServerStreamWriter<GetAllResponse> responseStream, ServerCallContext context)
    {
        var getAllResult = await _projectionsService.TryGetAll(
            request.ProjectionId.ToGuid(),
            request.ScopeId.ToGuid(),
            request.CallContext.ExecutionContext.ToExecutionContext(),
            context.CancellationToken).ConfigureAwait(false);

        if (!getAllResult.Success)
        {
            var response = new GetAllResponse
            {
                Failure = getAllResult.Exception.ToFailure(),
            };
            Log.SendingGetAllInBatchesFailed(_logger, request.ProjectionId, request.ScopeId, getAllResult.Exception);
            await responseStream.WriteAsync(response).ConfigureAwait(false);
            return;
        }

        var nextResponse = new GetAllResponse();
        foreach (var state in getAllResult.Result)
        {
            var stateMessage = state.ToProtobuf();

            if (nextResponse.States.Count == 0 || nextResponse.CalculateSize() + stateMessage.CalculateSize() < MaxBatchMessageSize)
            {
                nextResponse.States.Add(stateMessage);
                continue;
            }
            
            Log.SendingGetAllInBatchesResult(_logger, request.ProjectionId, request.ScopeId, nextResponse.States.Count);
            await responseStream.WriteAsync(nextResponse).ConfigureAwait(false);
            
            nextResponse = new GetAllResponse();
            nextResponse.States.Add(stateMessage);
        }
        
        if (nextResponse.States.Count > 0)
        {
            Log.SendingGetAllInBatchesResult(_logger, request.ProjectionId, request.ScopeId, nextResponse.States.Count);
            await responseStream.WriteAsync(nextResponse).ConfigureAwait(false);
        }
    }
}
