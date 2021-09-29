// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="ISubscriptions" />.
    /// </summary>
    [SingletonPerTenant]
    public class Subscriptions : ISubscriptions
    {
        readonly ConcurrentDictionary<SubscriptionId, ISubscription> _subscriptions = new();
        readonly MicroservicesConfiguration _microservicesConfiguration;
        readonly ISubscriptionFactory _subscriptionFactory;
        readonly IMetricsCollector _metrics;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscriptions"/> class.
        /// </summary>
        /// <param name="microservicesConfiguration">The configuration to use for finding the address of a producer Runtime from it's microservice id.</param>
        /// <param name="subscriptionFactory">The factory to use for creating subscriptions that subscribes to a producer microservice.</param>
        /// <param name="metrics">The system for collecting metrics.</param>
        /// <param name="logger">The logger.</param>
        public Subscriptions(
            MicroservicesConfiguration microservicesConfiguration,
            ISubscriptionFactory subscriptionFactory,
            IMetricsCollector metrics,
            ILogger logger)
        {
            _microservicesConfiguration = microservicesConfiguration;
            _subscriptionFactory = subscriptionFactory;
            _metrics = metrics;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<SubscriptionResponse> Subscribe(SubscriptionId subscriptionId)
        {
            _logger.SubscribingTo(subscriptionId);

            var producerMicroserviceId = subscriptionId.ProducerMicroserviceId;
            if (!TryGetProducerMicroserviceAddress(producerMicroserviceId, out var producerConnectionAddress))
            {
                _metrics.IncrementSubscriptionsMissingProducerMicroserviceAddress();
                _logger.NoMicroserviceConfigurationFor(producerMicroserviceId);
                return Task.FromResult(
                    SubscriptionResponse.Failed(
                        new Failure(
                            SubscriptionFailures.MissingMicroserviceConfiguration,
                            $"No microservice configuration for producer microservice {producerMicroserviceId}")));
            }

            var subscription = _subscriptions.GetOrAdd(subscriptionId, _ => _subscriptionFactory.Create(_, producerConnectionAddress));

            if (subscription.State == SubscriptionState.Created)
            {
                _metrics.IncrementTotalRegisteredSubscriptions();
                _logger.StartingCreatedSubscription(subscriptionId);
                subscription.Start();
            }
            else
            {
                _metrics.IncrementSubscriptionsAlreadyStarted();
                _logger.SubscriptionAlreadyRegistered(subscriptionId);
            }

            return subscription.ConnectionResponse;
        }

        bool TryGetProducerMicroserviceAddress(Microservice producerMicroservice, out MicroserviceAddress microserviceAddress)
        {
            microserviceAddress = default;
            if (!_microservicesConfiguration.TryGetValue(producerMicroservice, out var microserviceAddressConfig))
            {
                return false;
            }
            microserviceAddress = microserviceAddressConfig;
            return true;
        }
    }
}
