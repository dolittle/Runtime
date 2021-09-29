// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters.EventHorizon;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.Filters.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractFilterProcessor{TDefinition}" /> for processing events through a public events filter.
    /// </summary>
    public class PublicFilterProcessor : AbstractFilterProcessor<PublicFilterDefinition>
    {
        readonly IReverseCallDispatcher<PublicFilterClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse> _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFilterProcessor"/> class.
        /// </summary>
        /// <param name="definition">The <see cref="PublicFilterDefinition"/>.</param>
        /// <param name="dispatcher"><see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.</param>
        /// <param name="eventsToPublicStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PublicFilterProcessor(
            PublicFilterDefinition definition,
            IReverseCallDispatcher<PublicFilterClientToRuntimeMessage, FilterRuntimeToClientMessage, PublicFilterRegistrationRequest, FilterRegistrationResponse, FilterEventRequest, PartitionedFilterResponse> dispatcher,
            IWriteEventsToPublicStreams eventsToPublicStreamsWriter,
            ILogger logger)
            : base(ScopeId.Default, definition, eventsToPublicStreamsWriter, logger)
        {
            _dispatcher = dispatcher;
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, CancellationToken cancellationToken)
        {
            if (!@event.Public) return Task.FromResult<IFilterResult>(new SuccessfulFiltering(false, Guid.Empty));
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
            if (!@event.Public) return Task.FromResult<IFilterResult>(new SuccessfulFiltering(false, Guid.Empty));
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
