// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.Projections;

public class Projection : IProjection
{
    readonly ProjectionDefinition _projectionDefinition;
    readonly IReverseCallDispatcher<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse> _dispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="Projection"/> class.
    /// </summary>
    /// <param name="projectionDefinition">The <see cref="ProjectionDefinition" />.</param>
    /// <param name="dispatcher">The <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> dispatcher to use to send requests to the head.</param>
    public Projection(
        ProjectionDefinition projectionDefinition,
        IReverseCallDispatcher<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse> dispatcher)
    {
        _projectionDefinition = projectionDefinition;
        _dispatcher = dispatcher;
    }

    /// <inheritdoc/>
    public Task<IProjectionResult> Project(ProjectionCurrentState state, CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken)
        => Process(state, @event, partitionId, new ProjectionRequest(), cancellationToken);

    /// <inheritdoc/>
    public Task<IProjectionResult> Project(ProjectionCurrentState state, CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken)
    {
        var request = new ProjectionRequest
        {
            RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
        };
        return Process(state, @event, partitionId, request, cancellationToken);
    }

    async Task<IProjectionResult> Process(ProjectionCurrentState state, CommittedEvent @event, PartitionId partitionId, ProjectionRequest request, CancellationToken token)
    {
        request.Event = new Contracts.StreamEvent
        {
            Event = @event.ToProtobuf(),
            PartitionId = partitionId.Value,
            ScopeId = _projectionDefinition.Scope.ToProtobuf(),
        };
        request.CurrentState = state.ToProtobuf();

        var response = await _dispatcher.Call(request, token).ConfigureAwait(false);

        return response switch
        {
            { Failure: null, ResponseCase: ProjectionResponse.ResponseOneofCase.Replace } => new ProjectionReplaceResult(response.Replace.State),
            { Failure: null, ResponseCase: ProjectionResponse.ResponseOneofCase.Delete } => new ProjectionDeleteResult(),
            _ => new ProjectionFailedResult(response.Failure.Reason),
        };
    }
}