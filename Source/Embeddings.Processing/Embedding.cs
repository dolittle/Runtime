// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using ReverseCallDispatcherType = Dolittle.Runtime.Services.IReverseCallDispatcher<
    Dolittle.Runtime.Embeddings.Contracts.EmbeddingClientToRuntimeMessage,
    Dolittle.Runtime.Embeddings.Contracts.EmbeddingRuntimeToClientMessage,
    Dolittle.Runtime.Embeddings.Contracts.EmbeddingRegistrationRequest,
    Dolittle.Runtime.Embeddings.Contracts.EmbeddingRegistrationResponse,
    Dolittle.Runtime.Embeddings.Contracts.EmbeddingRequest,
    Dolittle.Runtime.Embeddings.Contracts.EmbeddingResponse>;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents an implementation of <see cref="IEmbedding" />.
/// </summary>
[DisableAutoRegistration]
public class Embedding : IEmbedding
{
    readonly EmbeddingId _embeddingId;
    readonly ReverseCallDispatcherType _dispatcher;
    readonly IEmbeddingRequestFactory _requestFactory;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes an instance of the <see cref="Embedding" /> class.
    /// </summary>
    /// <param name="embeddingId">The identifier for this embedding.</param>
    /// <param name="dispatcher">The reverse call dispatcher to use to dispatch update and delete calls to the client.</param>
    /// <param name="requestFactory">The factory to use to create requests to the client.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public Embedding(
        EmbeddingId embeddingId,
        ReverseCallDispatcherType dispatcher,
        IEmbeddingRequestFactory requestFactory,
        ILogger logger)
    {
        _embeddingId = embeddingId;
        _dispatcher = dispatcher;
        _requestFactory = requestFactory;
        _logger = logger;
    }

    /// <inheritdoc/> 
    public async Task<IProjectionResult> Project(ProjectionCurrentState state, UncommittedEvent @event, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        try
        {
            _logger.ProjectEventThroughDispatcher(_embeddingId, state);
            var response = await _dispatcher.Call(
                _requestFactory.Create(state, @event),
                executionContext,
                cancellationToken).ConfigureAwait(false);

            if (IsFailureResponse(response))
            {
                return new ProjectionFailedResult(GetFailureReason(response));
            }
            return response.ResponseCase switch
            {
                EmbeddingResponse.ResponseOneofCase.ProjectionDelete => new ProjectionDeleteResult(),
                EmbeddingResponse.ResponseOneofCase.ProjectionReplace => new ProjectionReplaceResult(response.ProjectionReplace.State),
                EmbeddingResponse.ResponseOneofCase.Compare
                    => new ProjectionFailedResult(new UnexpectedEmbeddingResponse(_embeddingId, response.ResponseCase)),
                EmbeddingResponse.ResponseOneofCase.Delete
                    => new ProjectionFailedResult(new UnexpectedEmbeddingResponse(_embeddingId, response.ResponseCase)),
                _ => new ProjectionFailedResult(new UnexpectedEmbeddingResponse(_embeddingId, response.ResponseCase))
            };
        }
        catch (Exception ex)
        {
            return new ProjectionFailedResult(ex);
        }
    }

    /// <inheritdoc/> 
    public async Task<Try<UncommittedEvents>> TryCompare(EmbeddingCurrentState currentState, ProjectionState desiredState, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.CompareStatesForEmbedding(_embeddingId, currentState, desiredState);
        var request = _requestFactory.TryCreate(currentState, desiredState);
        if (!request.Success)
        {
            return request.Exception;
        }
        return await DispatchAndHandleEmbeddingResponse(
            currentState,
            request,
            EmbeddingResponse.ResponseOneofCase.Compare,
            response => response.Compare.Events,
            (embedding, failureReason) => new EmbeddingRemoteCompareCallFailed(embedding, failureReason),
            executionContext,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/> 
    public async Task<Try<UncommittedEvents>> TryDelete(EmbeddingCurrentState currentState, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.DeletingStateForEmbedding(_embeddingId, currentState);
        var request = _requestFactory.TryCreate(currentState);
        if (!request.Success)
        {
            return request.Exception;
        }
        return await DispatchAndHandleEmbeddingResponse(
            currentState,
            request,
            EmbeddingResponse.ResponseOneofCase.Delete,
            response => response.Delete.Events,
            (embedding, failureReason) => new EmbeddingRemoteDeleteCallFailed(embedding, failureReason),
            executionContext,
            cancellationToken).ConfigureAwait(false);
    }

    async Task<Try<UncommittedEvents>> DispatchAndHandleEmbeddingResponse(
        EmbeddingCurrentState currentState,
        EmbeddingRequest request,
        EmbeddingResponse.ResponseOneofCase expectedResponse,
        Func<EmbeddingResponse, IEnumerable<Events.Contracts.UncommittedEvent>> getEvents,
        Func<EmbeddingId, string, Exception> createFailedException,
        ExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _dispatcher.Call(request, executionContext, cancellationToken).ConfigureAwait(false);
            if (IsFailureResponse(response))
            {
                return createFailedException(_embeddingId, GetFailureReason(response));
            }

            return response.ResponseCase == expectedResponse
                ? ToUncommittedEvents(currentState.Key.Value, getEvents(response))
                : new UnexpectedEmbeddingResponse(_embeddingId, response.ResponseCase);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }


    bool IsFailureResponse(EmbeddingResponse response)
        => response.Failure is not null || response.ProcessorFailure is not null;

    string GetFailureReason(EmbeddingResponse response) => response.ProcessorFailure?.Reason ?? response.Failure.Reason;

    UncommittedEvents ToUncommittedEvents(EventSourceId eventSourceId, IEnumerable<Events.Contracts.UncommittedEvent> events)
        => new(
            new ReadOnlyCollection<UncommittedEvent>(events.Select(_ =>
                new UncommittedEvent(
                    eventSourceId,
                    new Artifact(_.EventType.Id.ToGuid(),
                        _.EventType.Generation),
                    _.Public,
                    _.Content)).ToList()));
}
