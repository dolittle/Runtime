// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Services;

namespace Dolittle.Runtime.EventHorizon.Producer.Filter
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractFilterProcessor{TDefinition}" /> for processing events through a public events filter.
    /// </summary>
    public class PublicFilterProcessor : AbstractFilterProcessor<PublicFilterDefinition>
    {
        readonly IReverseCallDispatcher<PublicFilterClientToRuntimeResponse, PublicFilterRuntimeToClientRequest> _dispatcher;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFilterProcessor"/> class.
        /// </summary>
        /// <param name="definition">The <see cref="RemoteFilterDefinition"/>.</param>
        /// <param name="dispatcher"><see cref="IReverseCallDispatcher{TResponse, TRequest}"/>.</param>
        /// <param name="eventsToPublicStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PublicFilterProcessor(
            PublicFilterDefinition definition,
            IReverseCallDispatcher<PublicFilterClientToRuntimeResponse, PublicFilterRuntimeToClientRequest> dispatcher,
            IWriteEventsToPublicStreams eventsToPublicStreamsWriter,
            IExecutionContextManager executionContextManager,
            ILogger logger)
            : base(ScopeId.Default, definition, eventsToPublicStreamsWriter, logger)
        {
            _dispatcher = dispatcher;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        #nullable enable
        /// <inheritdoc/>
        public override async Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, RetryProcessingState? retryProcessingState, CancellationToken cancellationToken)
        {
            _logger.Debug($"Filter event that occurred @ {@event.Occurred} to public events stream '{Definition.TargetStream}'");
            if (!@event.Public) return new FilteringResult(false, PartitionId.NotSet);
            var request = new PublicFilterRuntimeToClientRequest
                {
                    Event = @event.ToProtobuf(),
                    Partition = partitionId.ToProtobuf(),
                    RetryProcessingState = retryProcessingState,
                    ExecutionContext = _executionContextManager.Current.ToByteString()
                };
            FilteringResult? result = null;
            await _dispatcher.Call(request, response => result = ResultFromResponse(response)).ConfigureAwait(false);
            return result!;
        }

        FilteringResult ResultFromResponse(PublicFilterClientToRuntimeResponse response)
        {
            if (response.Failed == null) return new FilteringResult(response.Success.IsIncluded, response.Success.Partition.To<PartitionId>());
            return new FilteringResult(response.Failed);
        }
    }
}