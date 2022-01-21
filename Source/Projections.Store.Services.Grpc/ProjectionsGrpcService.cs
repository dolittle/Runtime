// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Projections.Contracts;
using Dolittle.Runtime.Protobuf;
using Grpc.Core;
using static Dolittle.Runtime.Projections.Contracts.Projections;

namespace Dolittle.Runtime.Projections.Store.Services.Grpc;

/// <summary>
/// Represents the implementation of.
/// </summary>
public class ProjectionsGrpcService : ProjectionsBase
{
    readonly IProjectionsService _projectionsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionsGrpcService"/> class.
    /// </summary>
    /// <param name="projectionsService"><see cref="IProjectionsService"/>.</param>
    public ProjectionsGrpcService(
        IProjectionsService projectionsService)
    {
        _projectionsService = projectionsService;
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
            response.State = getOneResult.Result.ToProtobuf();
        else
            response.Failure = getOneResult.Exception.ToFailure();

        return response;
    }

    /// <inheritdoc/>
    public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var getOneResult = await _projectionsService.TryGetAll(
            request.ProjectionId.ToGuid(),
            request.ScopeId.ToGuid(),
            request.CallContext.ExecutionContext.ToExecutionContext(),
            context.CancellationToken).ConfigureAwait(false);

        if (getOneResult.Success)
            response.States.AddRange(getOneResult.Result.ToProtobuf());
        else
            response.Failure = getOneResult.Exception.ToFailure();

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

        var response = new GetAllResponse();
        if (!getAllResult.Success)
        {
            response.Failure = getAllResult.Exception.ToFailure();
            await responseStream.WriteAsync(response).ConfigureAwait(false);
            return;
        }

        foreach (var state in getAllResult.Result)
        {
            response.States.Add(state.ToProtobuf());

            if (response.CalculateSize() < 10) continue;
            
            await responseStream.WriteAsync(response).ConfigureAwait(false);
            response = new GetAllResponse();
        }

        if (response.States.Count > 0)
        {
            await responseStream.WriteAsync(response).ConfigureAwait(false);
        }
    }
}
