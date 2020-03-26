// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the handling of an event.
    /// </summary>
    /// <typeparam name="TResponse">The response <see cref="IMessage" /> type.</typeparam>
    /// <typeparam name="TRequest">The request <see cref="IMessage" /> type.</typeparam>
    public class EventProcessor<TResponse, TRequest> : IEventProcessor
        where TResponse : IMessage
        where TRequest : IMessage
    {
        readonly EventHandlerProcessingRequestHandler<TRequest, TResponse> _processingRequestHandler;
        readonly IExecutionContextManager _executionContextManager;
        readonly Func<CommittedEvent, PartitionId, Execution.ExecutionContext, ProcessingRequestProxy<TRequest>> _createProxy;
        readonly ILogger _logger;
        readonly string _logMessagePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessor{TResponse, TRequest}"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="id">The <see cref="EventProcessorId" />.</param>
        /// <param name="processingRequestHandler"><see cref="EventHandlerProcessingRequestHandler{TRequest, TResponse}"/> that handles the processing requests.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="createProxy">The callback for creating the proxy object.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessor(
            ScopeId scope,
            EventProcessorId id,
            EventHandlerProcessingRequestHandler<TRequest, TResponse> processingRequestHandler,
            IExecutionContextManager executionContextManager,
            Func<CommittedEvent, PartitionId, Dolittle.Execution.ExecutionContext, ProcessingRequestProxy<TRequest>> createProxy,
            ILogger logger)
        {
            Scope = scope;
            Identifier = id;
            _processingRequestHandler = processingRequestHandler;
            _executionContextManager = executionContextManager;
            _createProxy = createProxy;
            _logger = logger;
            _logMessagePrefix = $"Event Processor '{Identifier}'";
        }

        /// <inheritdoc />
        public ScopeId Scope { get; }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <inheritdoc />
        public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken = default)
        {
            _logger.Debug($"{_logMessagePrefix} is processing event '{@event.Type.Id.Value}' for partition '{partitionId.Value}'");

            return _processingRequestHandler.Process(_createProxy(@event, partitionId, _executionContextManager.Current));
        }
    }
}