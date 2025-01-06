// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using ReverseCallDispatcherType = Dolittle.Runtime.Services.IReverseCallDispatcher<
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerClientToRuntimeMessage,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRuntimeToClientMessage,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationRequest,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerRegistrationResponse,
                                    Dolittle.Runtime.Events.Processing.Contracts.HandleEventRequest,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="IEventProcessor" />that processes the handling of an event.
/// </summary>
public class EventProcessor : IEventProcessor
{
    readonly ReverseCallDispatcherType _dispatcher;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventProcessor"/> class.
    /// </summary>
    /// <param name="scope">The <see cref="ScopeId" />.</param>
    /// <param name="id">The <see cref="EventProcessorId" />.</param>
    /// <param name="dispatcher"><see cref="ReverseCallDispatcherType"/> dispatcher.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public EventProcessor(ScopeId scope, EventProcessorId id, ReverseCallDispatcherType dispatcher, ILogger logger)
    {
        Scope = scope;
        Identifier = id;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    /// <inheritdoc />
    public ScopeId Scope { get; }

    /// <inheritdoc />
    public EventProcessorId Identifier { get; }

    public CancellationToken? ShutdownToken => _dispatcher.ShutdownToken;

    public CancellationToken? DeadlineToken => _dispatcher.DeadlineToken;

    /// <inheritdoc />
    public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, StreamPosition position, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.EventProcessorIsProcessing(Identifier, @event.Type.Id, partitionId);

        var request = new HandleEventRequest
        {
            Event = new Contracts.StreamEvent { Event = @event.ToProtobuf(), PartitionId = partitionId.Value, ScopeId = Scope.ToProtobuf(), StreamPosition = position.Value },
        };
        return Process(request, executionContext, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, StreamPosition position, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.EventProcessorIsProcessingAgain(Identifier, @event.Type.Id, partitionId, retryCount, failureReason);
        var request = new HandleEventRequest
        {
            Event = new Contracts.StreamEvent { Event = @event.ToProtobuf(), PartitionId = partitionId.Value, ScopeId = Scope.ToProtobuf(), StreamPosition = position.Value },
            RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
        };
        return Process(request, executionContext, cancellationToken);
    }

    async Task<IProcessingResult> Process(HandleEventRequest request, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var response = await _dispatcher.Call(request, executionContext, cancellationToken).ConfigureAwait(false);

        return response switch
        {
            { Failure: null } => SuccessfulProcessing.Instance,
            _ => new FailedProcessing(response.Failure.Reason, response.Failure.Retry, response.Failure.RetryTimeout?.ToTimeSpan() ?? TimeSpan.MaxValue)
        };
    }
}
