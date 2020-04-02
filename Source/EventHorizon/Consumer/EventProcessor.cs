// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />.
    /// </summary>
    public class EventProcessor : IEventProcessor
    {
        readonly Subscription _subscription;
        readonly IWriteEventHorizonEvents _receivedEventsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessor"/> class.
        /// </summary>
        /// <param name="subscription">The <see cref="Subscription" />.</param>
        /// <param name="receivedEventsWriter">The <see cref="IWriteEventHorizonEvents" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessor(
            Subscription subscription,
            IWriteEventHorizonEvents receivedEventsWriter,
            ILogger logger)
        {
            Scope = subscription.Scope;
            Identifier = subscription.ProducerTenant.Value;
            _subscription = subscription;
            _receivedEventsWriter = receivedEventsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public ScopeId Scope { get; }

        /// <inheritdoc/>
        public EventProcessorId Identifier { get; }

        /// <inheritdoc/>
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, RetryProcessingState retryProcessingState, CancellationToken cancellationToken)
        {
            _logger.Trace($"Processing event '{@event.Type.Id}' in scope '{Scope}' from microservice '{_subscription.ProducerMicroservice}' and tenant '{_subscription.ProducerTenant}'");

            await _receivedEventsWriter.Write(@event, Scope, cancellationToken).ConfigureAwait(false);
            return new ProcessingResult();
        }
    }
}