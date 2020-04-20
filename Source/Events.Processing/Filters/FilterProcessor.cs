// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Services;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a default implementation of <see cref="AbstractFilterProcessor{T}"/> that processes a remote filter.
    /// </summary>
    public class FilterProcessor : AbstractFilterProcessor<RemoteFilterDefinition>
    {
        readonly IReverseCallDispatcher<FilterClientToRuntimeResponse, FilterRuntimeToClientRequest> _dispatcher;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterProcessor"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="definition">The <see cref="RemoteFilterDefinition"/>.</param>
        /// <param name="dispatcher">The <see cref="IReverseCallDispatcher{TResponse, TRequest}" />.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FilterProcessor(
            ScopeId scope,
            RemoteFilterDefinition definition,
            IReverseCallDispatcher<FilterClientToRuntimeResponse, FilterRuntimeToClientRequest> dispatcher,
            IWriteEventsToStreams eventsToStreamsWriter,
            IExecutionContextManager executionContextManager,
            ILogger logger)
            : base(scope, definition, eventsToStreamsWriter, logger)
        {
            _dispatcher = dispatcher;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        #nullable enable
        /// <inheritdoc/>
        public override async Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, RetryProcessingState? retryProcessingState, CancellationToken cancellationToken)
        {
            _logger.Debug($"Filter event that occurred @ {@event.Occurred}");

            var request = new FilterRuntimeToClientRequest
                {
                    Event = @event.ToProtobuf(),
                    Partition = partitionId.ToProtobuf(),
                    ExecutionContext = _executionContextManager.Current.ToByteString(),
                    RetryProcessingState = retryProcessingState
                };
            FilteringResult? result = null;
            await _dispatcher.Call(request, response => result = response).ConfigureAwait(false);
            return result!;
        }
    }
}