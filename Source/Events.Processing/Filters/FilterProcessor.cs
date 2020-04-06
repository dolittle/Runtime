// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Services;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a default implementation of <see cref="AbstractFilterProcessor{T}"/> that processes a remote filter.
    /// </summary>
    public class FilterProcessor : AbstractFilterProcessor<RemoteFilterDefinition>
    {
        readonly IReverseCallDispatcher<FiltersClientToRuntimeStreamMessage, FilterRuntimeToClientStreamMessage> _dispatcher;
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
            IReverseCallDispatcher<FiltersClientToRuntimeStreamMessage, FilterRuntimeToClientStreamMessage> dispatcher,
            IWriteEventsToStreams eventsToStreamsWriter,
            IExecutionContextManager executionContextManager,
            ILogger logger)
            : base(scope, definition, eventsToStreamsWriter, logger)
        {
            _dispatcher = dispatcher;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId)
        {
            _logger.Debug($"Filter event that occurred @ {@event.Occurred}");

            var request = new FilterEventRequest
                {
                    Event = @event.ToProtobuf(),
                    Partition = partitionId.ToProtobuf(),
                };

            return Filter(request);
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, string failureReason, uint retryCount)
        {
            _logger.Debug($"Filter event that occurred @ {@event.Occurred}");

            var request = new FilterEventRequest
                {
                    Event = @event.ToProtobuf(),
                    Partition = partitionId.ToProtobuf(),
                    RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
                };

            return Filter(request);
        }

        async Task<IFilterResult> Filter(FilterEventRequest request)
        {
            IFilterResult result = null;
            await _dispatcher.Call(
                new FilterRuntimeToClientStreamMessage
                {
                    FilterRequest = request,
                    ExecutionContext = _executionContextManager.Current.ToByteString()
                },
                response =>
                {
                    if (response.MessageCase != FiltersClientToRuntimeStreamMessage.MessageOneofCase.FilterResult)
                    {
                        result = response.FilterResult switch
                            {
                                { Failed: null } => new SuccessfulFiltering(response.FilterResult.Success.IsIncluded, response.FilterResult.Success.Partition.To<PartitionId>()),
                                _ => new FailedFiltering(response.FilterResult.Failed.Reason, response.FilterResult.Failed.Retry, response.FilterResult.Failed.RetryTimeout.ToTimeSpan())
                            };
                    }
                    else
                    {
                        result = new FailedFiltering("The response from the processing was of an unexpected response type.");
                    }
                }).ConfigureAwait(false);
            return result!;
        }
    }
}