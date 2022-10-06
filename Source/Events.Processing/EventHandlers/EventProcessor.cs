// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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
                                    Dolittle.Runtime.Events.Processing.Contracts.HandleEventsRequest,
                                    Dolittle.Runtime.Events.Processing.Contracts.EventHandlerResponse>;

using StreamEvent = Dolittle.Runtime.Events.Store.Streams.StreamEvent;

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

    /// <inheritdoc />
    public Task<IProcessingResult> Process(IReadOnlyList<StreamEvent> batch, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.EventProcessorIsProcessingBatch(Identifier, Scope, batch.Count);

        var request = new HandleEventsRequest
        {
            Batch = { batch.Select(_ => _.ToProtobuf(Scope))}
        };
        return Process(request, executionContext, cancellationToken);
    }
    
    /// <inheritdoc />
    public Task<IProcessingResult> Process(StreamEvent streamEvent, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.EventProcessorIsProcessing(Identifier, streamEvent.Event.Type.Id, streamEvent.Partition);

        var request = new HandleEventsRequest
        {
            Event = streamEvent.ToProtobuf(Scope)
        };
        return Process(request, executionContext, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IProcessingResult> ReProcess(StreamEvent streamEvent, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.EventProcessorIsProcessingAgain(Identifier, streamEvent.Event.Type.Id, streamEvent.Partition, retryCount, failureReason);
        var request = new HandleEventsRequest
        {
            Event = streamEvent.ToProtobuf(Scope),
            RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
        };
        return Process(request, executionContext, cancellationToken);
    }
    /// <inheritdoc/>
    public Task<IProcessingResult> ReProcess(IReadOnlyList<StreamEvent> batch, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.EventProcessorIsProcessingBatchAgain(Identifier, Scope, batch.Count, retryCount, failureReason);
        var request = new HandleEventsRequest
        {
            Batch =  { batch.Select(_ => _.ToProtobuf(Scope))},
            RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
        };
        return Process(request, executionContext, cancellationToken);
    }

    async Task<IProcessingResult> Process(HandleEventsRequest request, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var response = await _dispatcher.Call(request, executionContext, cancellationToken).ConfigureAwait(false);

        return response switch
        {
            { Failure: null } => new SuccessfulProcessing(),
            _ => new FailedProcessing(response.Failure.Reason, request.Event.Metadata.StreamPosition, request.Event.Metadata.PartitionId, response.Failure.Retry, response.Failure.RetryTimeout?.ToTimeSpan() ?? TimeSpan.MaxValue)
        };
    }
}
