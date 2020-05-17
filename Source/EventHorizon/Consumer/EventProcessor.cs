// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />.
    /// </summary>
    public class EventProcessor : IEventProcessor
    {
        readonly ConsentId _consentId;
        readonly SubscriptionId _subscriptionId;
        readonly IWriteEventHorizonEvents _receivedEventsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessor"/> class.
        /// </summary>
        /// <param name="consentId">THe <see cref="ConsentId" />.</param>
        /// <param name="subscription">The <see cref="Subscription" />.</param>
        /// <param name="receivedEventsWriter">The <see cref="IWriteEventHorizonEvents" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessor(
            ConsentId consentId,
            SubscriptionId subscription,
            IWriteEventHorizonEvents receivedEventsWriter,
            ILogger logger)
        {
            _consentId = consentId;
            Scope = subscription.ScopeId;
            Identifier = subscription.ProducerTenantId.Value;
            _subscriptionId = subscription;
            _receivedEventsWriter = receivedEventsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public ScopeId Scope { get; }

        /// <inheritdoc/>
        public EventProcessorId Identifier { get; }

        /// <inheritdoc/>
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken)
        {
            _logger.Trace($"Processing event '{@event.Type.Id}' in scope '{Scope}' from microservice '{_subscriptionId.ProducerMicroserviceId}' and tenant '{_subscriptionId.ProducerTenantId}'");

            await _receivedEventsWriter.Write(@event, _consentId, Scope, cancellationToken).ConfigureAwait(false);
            return new SuccessfulProcessing();
        }

        /// <inheritdoc/>
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken)
        {
            _logger.Trace($"Processing event '{@event.Type.Id}' in scope '{Scope}' from microservice '{_subscriptionId.ProducerMicroserviceId}' and tenant '{_subscriptionId.ProducerTenantId}'");

            await _receivedEventsWriter.Write(@event, _consentId,  Scope, cancellationToken).ConfigureAwait(false);
            return new SuccessfulProcessing();
        }
    }
}