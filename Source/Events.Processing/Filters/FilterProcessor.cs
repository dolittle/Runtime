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

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a default implementation of <see cref="AbstractFilterProcessor{T}"/> that processes a remote filter.
    /// </summary>
    /// <typeparam name="TResponse">The response <see cref="IMessage" /> type.</typeparam>
    /// <typeparam name="TRequest">The request <see cref="IMessage" /> type.</typeparam>
    public class FilterProcessor<TResponse, TRequest> : AbstractFilterProcessor<RemoteFilterDefinition>
        where TResponse : IMessage
        where TRequest : IMessage
    {
        readonly FilterRequestHandler<TRequest, TResponse> _requestHandler;
        readonly IExecutionContextManager _executionContextManager;
        readonly Func<CommittedEvent, PartitionId, Execution.ExecutionContext, ProcessingRequestProxy<TRequest>> _createProxy;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterProcessor{TResponse, TRequest}"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="definition">The <see cref="RemoteFilterDefinition"/>.</param>
        /// <param name="requestHandler"><see cref="FilterRequestHandler{TRequest, TResponse}"/> for handling filter requests.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="createProxy">The callback to create the <see cref="ProcessingRequestProxy{TRequest}" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FilterProcessor(
            ScopeId scope,
            RemoteFilterDefinition definition,
            FilterRequestHandler<TRequest, TResponse> requestHandler,
            IWriteEventsToStreams eventsToStreamsWriter,
            IExecutionContextManager executionContextManager,
            Func<CommittedEvent, PartitionId, Dolittle.Execution.ExecutionContext, ProcessingRequestProxy<TRequest>> createProxy,
            ILogger logger)
            : base(scope, definition, eventsToStreamsWriter, logger)
        {
            _requestHandler = requestHandler;
            _executionContextManager = executionContextManager;
            _createProxy = createProxy;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, CancellationToken cancellationToken)
        {
            _logger.Debug($"Filter event that occurred @ {@event.Occurred}");
            return _requestHandler.Process(_createProxy(@event, partitionId, _executionContextManager.Current));
        }
    }
}