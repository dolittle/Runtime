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
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Projections;

public class Projection : IProjection
{
    readonly IReverseCallDispatcher<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse> _dispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="Projection"/> class.
    /// </summary>
    /// <param name="dispatcher">The <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> dispatcher to use to send requests to the head.</param>
    /// <param name="projectionDefinition">The <see cref="ProjectionDefinition" />.</param>
    /// <param name="alias">The alias of the Projection (if provided) from the Client.</param>
    /// <param name="hasAlias">A value indicating whether an alias was provided by the Client.</param>
    public Projection(
        IReverseCallDispatcher<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse> dispatcher,
        ProjectionDefinition projectionDefinition,
        ProjectionAlias alias,
        bool hasAlias)
    {
        Definition = projectionDefinition;
        _dispatcher = dispatcher;
        Alias = alias;
        HasAlias = hasAlias;
    }

    /// <inheritdoc/>
    public ProjectionDefinition Definition { get; }

    /// <inheritdoc />
    public ProjectionAlias Alias { get; }

    /// <inheritdoc />
    public bool HasAlias { get; }

    /// <inheritdoc/>
    public Task<IProjectionResult> Project(ProjectionCurrentState state, CommittedEvent @event, PartitionId partitionId, ExecutionContext executionContext, CancellationToken cancellationToken)
        => Process(state, @event, partitionId, new ProjectionRequest(), executionContext, cancellationToken);

    /// <inheritdoc/>
    public Task<IProjectionResult> Project(ProjectionCurrentState state, CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var request = new ProjectionRequest
        {
            RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
        };
        return Process(state, @event, partitionId, request, executionContext, cancellationToken);
    }

    async Task<IProjectionResult> Process(ProjectionCurrentState state, CommittedEvent @event, PartitionId partitionId, ProjectionRequest request, ExecutionContext executionContext, CancellationToken token)
    {
        request.Event = new Contracts.StreamEvent
        {
            Event = @event.ToProtobuf(),
            PartitionId = partitionId.Value,
            ScopeId = Definition.Scope.ToProtobuf(),
        };
        request.CurrentState = state.ToProtobuf();

        var response = await _dispatcher.Call(request, executionContext, token).ConfigureAwait(false);

        return response switch
        {
            { Failure: null, ResponseCase: ProjectionResponse.ResponseOneofCase.Replace } => new ProjectionReplaceResult(response.Replace.State),
            { Failure: null, ResponseCase: ProjectionResponse.ResponseOneofCase.Delete } => new ProjectionDeleteResult(),
            _ => new ProjectionFailedResult(response.Failure.Reason, response.Failure.Retry, response.Failure.RetryTimeout.ToTimeSpan() )
        };
    }
}
