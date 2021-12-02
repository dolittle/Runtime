// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Embeddings.Processing;
using Grpc.Core;
using static Dolittle.Runtime.Embeddings.Contracts.EmbeddingStore;
using System.Linq;
using Dolittle.Runtime.Rudimentary;
using System;
using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store.Services.Grpc;

/// <summary>
/// Represents the implementation of the <see cref="EmbeddingStoreBase" /> grpc service.
/// </summary>
public class EmbeddingStoreGrpcService : EmbeddingStoreBase
{
    readonly IEmbeddingsService _embeddingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingStoreGrpcService"/> class.
    /// </summary>
    /// <param name="embeddingService"><see cref="IEmbeddingsService"/>.</param>
    public EmbeddingStoreGrpcService(IEmbeddingsService embeddingService)
    {
        _embeddingsService = embeddingService;
    }

    /// <inheritdoc/>
    public override async Task<Contracts.GetOneResponse> GetOne(Contracts.GetOneRequest request, ServerCallContext context)
        => CreateResponse<Contracts.GetOneResponse, EmbeddingCurrentState>(
            await _embeddingsService.TryGetOne(
                request.EmbeddingId.ToGuid(),
                request.Key,
                request.CallContext.ExecutionContext.ToExecutionContext(),
                context.CancellationToken).ConfigureAwait(false),
            (result, response) => response.State = result.ToProtobuf(),
            (failure, response) => response.Failure = failure);

    /// <inheritdoc/>
    public override async Task<Contracts.GetAllResponse> GetAll(Contracts.GetAllRequest request, ServerCallContext context)
        => CreateResponse<Contracts.GetAllResponse, IEnumerable<EmbeddingCurrentState>>(
            await _embeddingsService.TryGetAll(
                request.EmbeddingId.ToGuid(),
                request.CallContext.ExecutionContext.ToExecutionContext(),
                context.CancellationToken).ConfigureAwait(false),
            (result, response) => response.States.AddRange(result.ToProtobuf()),
            (failure, response) => response.Failure = failure);

    /// <inheritdoc/>
    public override async Task<Contracts.GetKeysResponse> GetKeys(Contracts.GetKeysRequest request, ServerCallContext context)
        => CreateResponse<Contracts.GetKeysResponse, IEnumerable<ProjectionKey>>(
            await _embeddingsService.TryGetKeys(
                request.EmbeddingId.ToGuid(),
                request.CallContext.ExecutionContext.ToExecutionContext(),
                context.CancellationToken).ConfigureAwait(false),
            (result, response) => response.Keys.AddRange(result.Select(_ => _.Value)),
            (failure, response) => response.Failure = failure);

    TResponse CreateResponse<TResponse, TResult>(
        Try<TResult> maybeResult,
        Action<TResult, TResponse> onSuccess,
        Action<Failure, TResponse> onFailure)
        where TResponse : new()
    {
        var response = new TResponse();
        if (maybeResult.Success)
        {
            onSuccess(maybeResult, response);
        }
        else
        {
            onFailure(maybeResult.Exception.ToFailure(), response);
        }
        return response;
    }
}