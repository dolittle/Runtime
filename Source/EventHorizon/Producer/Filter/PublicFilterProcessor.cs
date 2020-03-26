// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.EventHorizon.Producer.Filter
{
    /// <summary>
    /// Represents an implementation of <see cref="AbstractFilterProcessor{TDefinition}" /> for processing events through a public events filter.
    /// </summary>
    public class PublicFilterProcessor : AbstractFilterProcessor<RemoteFilterDefinition>
    {
        readonly FilterRequestHandler<PublicFilterRuntimeToClientRequest, PublicFilterClientToRuntimeResponse> _requestHandler;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicFilterProcessor"/> class.
        /// </summary>
        /// <param name="definition">The <see cref="RemoteFilterDefinition"/>.</param>
        /// <param name="requestHandler"><see cref="FilterRequestHandler{TRequest, TResponse}"/> for handling filter requests.</param>
        /// <param name="eventsToPublicStreamsWriter">The <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public PublicFilterProcessor(
            RemoteFilterDefinition definition,
            FilterRequestHandler<PublicFilterRuntimeToClientRequest, PublicFilterClientToRuntimeResponse> requestHandler,
            IWriteEventsToPublicStreams eventsToPublicStreamsWriter,
            IExecutionContextManager executionContextManager,
            ILogger logger)
            : base(ScopeId.Default, definition, eventsToPublicStreamsWriter, logger)
        {
            _requestHandler = requestHandler;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, CancellationToken cancellationToken)
        {
            _logger.Debug($"Filter event that occurred @ {@event.Occurred} to public events stream '{Definition.TargetStream}'");
            if (!@event.Public) return Task.FromResult<IFilterResult>(new SucceededFilteringResult(false, PartitionId.NotSet));
            return _requestHandler.Process(new PublicFilterRequestProxy(@event, partitionId, _executionContextManager.Current));
        }
    }
}