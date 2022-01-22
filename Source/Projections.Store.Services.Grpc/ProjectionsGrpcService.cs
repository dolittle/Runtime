// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
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
            var states = await getAllResult.Result.ToListAsync(context.CancellationToken).ConfigureAwait(false);
            response.States.AddRange(states.ToProtobuf());
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
            Log.SendingGetAllInBatchesFailed(_logger, request.ProjectionId, request.ScopeId, getAllResult.Exception);
            var response = new GetAllResponse { Failure = getAllResult.Exception.ToFailure() };
            await responseStream.WriteAsync(response).ConfigureAwait(false);
            return;
        }

        var batchToSend = new GetAllResponse();
        var statesEnumerator = getAllResult.Result.GetAsyncEnumerator(context.CancellationToken);
        try
        {
            while (true)
            {
                var (batchOverflow, overflowState) = await FillUpBatch(statesEnumerator, batchToSend).ConfigureAwait(false);
                if (BatchHasStatesToSend(batchToSend))
                {
                    await SendBatch(request, batchToSend, responseStream, _logger).ConfigureAwait(false);
                    batchToSend = new GetAllResponse();
                    if (batchOverflow)
                    {
                        batchToSend.States.Add(overflowState);
                    }
                }
                else if (batchOverflow && !BatchHasStatesToSend(batchToSend))
                {
                    await SendTooBigState(overflowState, request, batchToSend, responseStream, _logger).ConfigureAwait(false);
                    batchToSend = new GetAllResponse();
                }
                else
                {
                    break;
                }
            }
        }
        finally
        {
            await statesEnumerator.DisposeAsync().ConfigureAwait(false);
        }
    }

    static async Task<(bool batchOverflow, ProjectionCurrentState overflowState)> FillUpBatch(IAsyncEnumerator<Projections.Store.State.ProjectionCurrentState> states, GetAllResponse batch)
    {
        while (await states.MoveNextAsync().ConfigureAwait(false))
        {
            var nextState = states.Current.ToProtobuf();
            if (BatchWouldBeTooLarge(batch, nextState))
            {
                return (true, nextState);
            }
            batch.States.Add(nextState);
        }
        return (false, default);
    }

    static bool BatchWouldBeTooLarge(GetAllResponse batch, ProjectionCurrentState nextState)
        => batch.CalculateSize() + nextState.CalculateSize() > MaxBatchMessageSize;

    static bool BatchHasStatesToSend(GetAllResponse batch) => batch.States.Count > 0;
    
    static Task SendTooBigState(ProjectionCurrentState stateThatDoesNotFitBatch, GetAllRequest request, GetAllResponse batchToSend, IServerStreamWriter<GetAllResponse> responseStream, ILogger logger)
    {
        Log.ProjectionStateTooLargeButSendingAnyways(
            logger,
            stateThatDoesNotFitBatch.Key,
            request.ProjectionId,
            request.ScopeId,
            stateThatDoesNotFitBatch.CalculateSize(),
            MaxBatchMessageSize);
        batchToSend.States.Add(stateThatDoesNotFitBatch);
        return SendBatch(request, batchToSend, responseStream, logger);
    }
    
    static Task SendBatch(GetAllRequest request, GetAllResponse batchToSend, IServerStreamWriter<GetAllResponse> responseStream, ILogger logger)
    {
        Log.SendingGetAllInBatchesResult(logger, request.ProjectionId, request.ScopeId, batchToSend.States.Count);
        return responseStream.WriteAsync(batchToSend);
    }
}
