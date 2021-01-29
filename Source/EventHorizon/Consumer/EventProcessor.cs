// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;

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
        readonly IAsyncPolicyFor<EventProcessor> _policy;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessor"/> class.
        /// </summary>
        /// <param name="consentId">THe <see cref="ConsentId" />.</param>
        /// <param name="subscription">The <see cref="Subscription" />.</param>
        /// <param name="receivedEventsWriter">The <see cref="IWriteEventHorizonEvents" />.</param>
        /// <param name="policy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="EventProcessor" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessor(
            ConsentId consentId,
            SubscriptionId subscription,
            IWriteEventHorizonEvents receivedEventsWriter,
            IAsyncPolicyFor<EventProcessor> policy,
            ILogger logger)
        {
            _consentId = consentId;
            Scope = subscription.ScopeId;
            Identifier = subscription.ProducerTenantId.Value;
            _subscriptionId = subscription;
            _receivedEventsWriter = receivedEventsWriter;
            _policy = policy;
            _logger = logger;
        }

        /// <inheritdoc/>
        public ScopeId Scope { get; }

        /// <inheritdoc/>
        public EventProcessorId Identifier { get; }

        /// <inheritdoc/>
        public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken) => Process(@event, cancellationToken);

        /// <inheritdoc/>
        public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Retrying processing of Event from Event Horizon");
            return Process(@event, cancellationToken);
        }

        async Task<IProcessingResult> Process(CommittedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogTrace(
                "Processing Event {EventType} from Event Horizon in Scope {Scope} from Microservice {ProducerMicroservice} and Tenant {ProducerTenant}",
                @event.Type.Id,
                Scope,
                _subscriptionId.ProducerMicroserviceId,
                _subscriptionId.ProducerTenantId);

            await _policy.Execute(
                cancellationToken => _receivedEventsWriter.Write(@event, _consentId, Scope, cancellationToken),
                cancellationToken).ConfigureAwait(false);
            return new SuccessfulProcessing();
        }
    }
}
