// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Services;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractFilterProcessor{TDefinition}" /> for processing events through a public events filter.
    /// </summary>
    public class PublicFilterProcessor : AbstractFilterProcessor<PublicFilterDefinition>
    {
        readonly IReverseCallDispatcher<Contracts.PublicFiltersClientToRuntimeMessage, Contracts.FilterRuntimeToClientMessage> _dispatcher;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFilterProcessor"/> class.
        /// </summary>
        /// <param name="definition">The <see cref="RemoteFilterDefinition"/>.</param>
        /// <param name="dispatcher"><see cref="IReverseCallDispatcher{TResponse, TRequest}"/>.</param>
        /// <param name="eventsToPublicStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="ExecutionContext"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PublicFilterProcessor(
            PublicFilterDefinition definition,
            IReverseCallDispatcher<Contracts.PublicFiltersClientToRuntimeMessage, Contracts.FilterRuntimeToClientMessage> dispatcher,
            IWriteEventsToPublicStreams eventsToPublicStreamsWriter,
            IExecutionContextManager executionContextManager,
            ILogger logger)
            : base(ScopeId.Default, definition, eventsToPublicStreamsWriter, logger)
        {
            _dispatcher = dispatcher;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId)
        {
            _logger.Debug($"Filter event that occurred @ {@event.Occurred} to public events stream '{Definition.TargetStream}'");
            if (!@event.Public) return Task.FromResult<IFilterResult>(new SuccessfulFiltering(false, PartitionId.NotSet));
            var request = new Contracts.FilterEventRequest
                {
                    Event = @event.ToProtobuf(),
                    PartitionId = partitionId.ToProtobuf(),
                };

            return Filter(request);
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, string failureReason, uint retryCount)
        {
            _logger.Debug($"Filter event that occurred @ {@event.Occurred} to public events stream '{Definition.TargetStream}'");
            if (!@event.Public) return Task.FromResult<IFilterResult>(new SuccessfulFiltering(false, PartitionId.NotSet));
            var request = new Contracts.FilterEventRequest
                {
                    Event = @event.ToProtobuf(),
                    PartitionId = partitionId.ToProtobuf(),
                    RetryProcessingState = new Contracts.RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
                };

            return Filter(request);
        }

        async Task<IFilterResult> Filter(Contracts.FilterEventRequest request)
        {
            request.CallContext = new Dolittle.Services.Contracts.ReverseCallRequestContext { ExecutionContext = _executionContextManager.Current.ToProtobuf() };
            var response = await _dispatcher.Call(new Contracts.FilterRuntimeToClientMessage { FilterRequest = request }).ConfigureAwait(false);
            if (response.MessageCase == Contracts.PublicFiltersClientToRuntimeMessage.MessageOneofCase.FilterResult)
            {
                return response.FilterResult switch
                    {
                        { Failure: null } => new SuccessfulFiltering(response.FilterResult.IsIncluded, response.FilterResult.PartitionId.To<PartitionId>()),
                        _ => new FailedFiltering(response.FilterResult.Failure.Reason, response.FilterResult.Failure.Retry, response.FilterResult.Failure.RetryTimeout.ToTimeSpan())
                    };
            }

            return new FailedFiltering("The response from the processing was of an unexpected response type.");
        }
    }
}