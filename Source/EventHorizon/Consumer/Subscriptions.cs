// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Lifecycle;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using System.Threading.Tasks;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="ISubscriptions" />.
    /// </summary>
    [SingletonPerTenant]
    public class Subscriptions : ISubscriptions
    {
        readonly ConcurrentDictionary<SubscriptionId, Subscription> _subscriptions = new();
        readonly ILoggerFactory _loggerFactory;
        readonly IStreamProcessorFactory _streamProcessorFactory;

        readonly MicroservicesConfiguration _microservicesConfiguration;
        readonly IEstablishEventHorizonConnections _eventHorizonConnectionEstablisher;
        readonly ILogger<Subscriptions> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriptions"/> class.
        /// </summary>
        /// <param name="streamProcessorStates">The <see cref="IResilientStreamProcessorStateRepository" />.</param>
        /// <param name="eventsFetcherPolicy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="ICanFetchEventsFromStream" />.</param>
        /// <param name="streamProcessorFactory">The <see cref="IStreamProcessorFactory" />.</param>
        /// <param name="microservicesConfiguration">The <see cref="MicroservicesConfiguration" />.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        public Subscriptions(
            IStreamProcessorFactory streamProcessorFactory,
            MicroservicesConfiguration microservicesConfiguration,
            IEstablishEventHorizonConnections eventHorizonConnectionEstablisher,
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _streamProcessorFactory = streamProcessorFactory;
            _microservicesConfiguration = microservicesConfiguration;
            _eventHorizonConnectionEstablisher = eventHorizonConnectionEstablisher;
            _logger = loggerFactory.CreateLogger<Subscriptions>();
        }

        /// <inheritdoc/>
        public bool TryGetConsentFor(SubscriptionId subscriptionId, out ConsentId consentId)
        {
            var result = _subscriptions.TryGetValue(subscriptionId, out var subscription);
            consentId = subscription?.Consent;
            return result;
        }

        /// <inheritdoc />
        public async Task<SubscriptionResponse> Subscribe(SubscriptionId subscriptionId, CancellationToken cancellationToken)
        {
            if (_subscriptions.ContainsKey(subscriptionId))
            {
                _logger.SubscriptionAlreadyRegistered(subscriptionId);
                return SubscriptionResponse.Succeeded(_subscriptions[subscriptionId].Consent);
            }

            var producerMicroservice = subscriptionId.ProducerMicroserviceId;
            if (!TryGetMicroserviceAddress(producerMicroservice, out var connectionAddress))
            {
                _logger.NoMicroserviceConfigurationFor(producerMicroservice);
                return SubscriptionResponse.Failed(new Failure(
                    SubscriptionFailures.MissingMicroserviceConfiguration,
                    $"No microservice configuration for producer mircoservice {producerMicroservice}"));
            }

            var subscription = new Subscription(
                subscriptionId,
                connectionAddress,
                _streamProcessorFactory,
                _eventHorizonConnectionEstablisher,
                _loggerFactory);
            if (!_subscriptions.TryAdd(subscriptionId, subscription))
            {
                _logger.SubscriptionAlreadyRegistered(subscriptionId);
                return SubscriptionResponse.Succeeded(_subscriptions[subscriptionId].Consent);
            }

            var response = await subscription.Register().ConfigureAwait(false);
            if (!response.Success)
            {
                _subscriptions.TryRemove(subscriptionId, out var _);
            }
            return response;
        }

        bool TryGetMicroserviceAddress(Microservice producerMicroservice, out MicroserviceAddress microserviceAddress)
        {
            var result = _microservicesConfiguration.TryGetValue(producerMicroservice, out var microserviceAddressConfig);
            microserviceAddress = microserviceAddressConfig;
            return result;
        }
    }
}
