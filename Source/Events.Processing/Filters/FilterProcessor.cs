// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Services;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a default implementation of <see cref="AbstractFilterProcessor"/>.
    /// </summary>
    public class FilterProcessor : AbstractFilterProcessor
    {
        readonly IReverseCallDispatcher<FilterClientToRuntimeResponse, FilterRuntimeToClientRequest> _callDispatcher;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterProcessor"/> class.
        /// </summary>
        /// <param name="eventProcessorId"><see cref="EventProcessorId"/> for the event processor.</param>
        /// <param name="callDispatcher"><see cref="IReverseCallDispatcher{TResponse, TRequest}"/> for server requests.</param>
        /// <param name="targetStreamId"><see cref="StreamId"/> to write to after filtering.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FilterProcessor(
            EventProcessorId eventProcessorId,
            IReverseCallDispatcher<FilterClientToRuntimeResponse, FilterRuntimeToClientRequest> callDispatcher,
            StreamId targetStreamId,
            IWriteEventsToStreams eventsToStreamsWriter,
            IExecutionContextManager executionContextManager,
            ILogger logger)
            : base(eventProcessorId, targetStreamId, eventsToStreamsWriter, logger)
        {
            _callDispatcher = callDispatcher;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<IFilterResult> Filter(Store.CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId)
        {
            var message = new FilterRuntimeToClientRequest
            {
                Event = @event.ToProtobuf(),
                ExecutionContext = _executionContextManager.Current.ToByteString()
            };

            _logger.Debug($"Filter event that occurred @ {@event.Occurred}");
            FilterClientToRuntimeResponse result = null;
            await _callDispatcher.Call(message, response => result = response).ConfigureAwait(false);
            _logger.Debug($"Filter result : {result.IsIncluded}");
            return new SucceededFilteringResult(true, partitionId);
        }
    }
}