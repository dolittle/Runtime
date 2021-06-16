// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.Filters.Partitioned
{
    /// <summary>
    /// Represents a default implementation of <see cref="AbstractFilterProcessor{T}"/> that processes a remote filter.
    /// </summary>
    public class FilterProcessor : AbstractFilterProcessor<FilterDefinition>
    {
        readonly IReverseCallDispatcher<PartitionedFilterClientToRuntimeMessage, FilterRuntimeToClientMessage, PartitionedFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse> _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterProcessor"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="definition">The <see cref="FilterDefinition"/>.</param>
        /// <param name="dispatcher">The <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" />.</param>
        /// <param name="eventsToStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FilterProcessor(
            ScopeId scope,
            FilterDefinition definition,
            IReverseCallDispatcher<PartitionedFilterClientToRuntimeMessage, FilterRuntimeToClientMessage, PartitionedFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse> dispatcher,
            IWriteEventsToStreams eventsToStreamsWriter,
            ILogger logger)
            : base(scope, definition, eventsToStreamsWriter, logger)
        {
            _dispatcher = dispatcher;
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, CancellationToken cancellationToken)
        {
            var request = new FilterEventRequest
            {
                Event = @event.ToProtobuf(),
                ScopeId = Scope.ToProtobuf()
            };

            return Filter(request, cancellationToken);
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, string failureReason, uint retryCount, CancellationToken cancellationToken)
        {
            var request = new FilterEventRequest
            {
                Event = @event.ToProtobuf(),
                ScopeId = Scope.ToProtobuf(),
                RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
            };

            return Filter(request, cancellationToken);
        }

        async Task<IFilterResult> Filter(FilterEventRequest request, CancellationToken cancellationToken)
        {
            var response = await _dispatcher.Call(request, cancellationToken).ConfigureAwait(false);
            return response switch
            {
                { Failure: null } => new SuccessfulFiltering(response.IsIncluded, response.PartitionId.ToGuid()),
                _ => new FailedFiltering(response.Failure.Reason, response.Failure.Retry, response.Failure.RetryTimeout.ToTimeSpan())
            };
        }
    }
}
