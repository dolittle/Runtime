// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.EventHorizon.Consumer.Connections;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Resilience;
using Microsoft.Extensions.Logging;

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
        readonly IEventHorizonConnectionFactory _eventHorizonConnectionFactory;
        readonly IAsyncPolicyFor<Subscription> _subscriptionPolicy;
        readonly IGetNextEventToReceiveForSubscription _subscriptionPositions;
        readonly ILogger<Subscriptions> _logger;

        public Subscriptions(
            IStreamProcessorFactory streamProcessorFactory,
            MicroservicesConfiguration microservicesConfiguration,
            IEventHorizonConnectionFactory eventHorizonConnectionFactory,
            IAsyncPolicyFor<Subscription> subscriptionPolicy,
            IGetNextEventToReceiveForSubscription subscriptionPositions,
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _streamProcessorFactory = streamProcessorFactory;
            _microservicesConfiguration = microservicesConfiguration;
            _eventHorizonConnectionFactory = eventHorizonConnectionFactory;
            _subscriptionPolicy = subscriptionPolicy;
            _subscriptionPositions = subscriptionPositions;
            _logger = loggerFactory.CreateLogger<Subscriptions>();
        }

        /// <inheritdoc />
        public Task<SubscriptionResponse> Subscribe(SubscriptionId subscriptionId)
        {
            _logger.SubscribingTo(subscriptionId);

            var producerMicroserviceId = subscriptionId.ProducerMicroserviceId;
            if (!TryGetProducerMicroserviceAddress(producerMicroserviceId, out var producerConnectionAddress))
            {
                _logger.NoMicroserviceConfigurationFor(producerMicroserviceId);
                return Task.FromResult(
                    SubscriptionResponse.Failed(
                        new Failure(
                            SubscriptionFailures.MissingMicroserviceConfiguration,
                            $"No microservice configuration for producer microservice {producerMicroserviceId}")));
            }

            var subscription = _subscriptions.GetOrAdd(subscriptionId, CreateNewSubscription(subscriptionId, producerConnectionAddress));

            if (subscription.State == SubscriptionState.Created)
            {
                _logger.StartingCreatedSubscription(subscriptionId);
                subscription.Start();
            }
            else
            {
                _logger.SubscriptionAlreadyRegistered(subscriptionId);
            }

            return subscription.ConnectionResponse;
        }

        bool TryGetProducerMicroserviceAddress(Microservice producerMicroservice, out MicroserviceAddress microserviceAddress)
        {
            var result = _microservicesConfiguration.TryGetValue(producerMicroservice, out var microserviceAddressConfig);
            microserviceAddress = microserviceAddressConfig;
            return result;
        }

        Subscription CreateNewSubscription(SubscriptionId subscriptionId, MicroserviceAddress producerMicroserviceAddress)
            => new(
                subscriptionId,
                producerMicroserviceAddress,
                _subscriptionPolicy,
                _eventHorizonConnectionFactory,
                _streamProcessorFactory,
                _subscriptionPositions,
                _loggerFactory.CreateLogger<Subscription>());
    }
}
