// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.EventHorizon;
using Dolittle.Applications;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;
using Grpc.Core;
using static contracts::Dolittle.Runtime.EventHorizon.Subscriptions;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    [Singleton]
    public class SubscriptionsService : SubscriptionsBase
    {
        readonly IConsumerClient _consumerClient;
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly IMicroservices _microservices;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsService"/> class.
        /// </summary>
        /// <param name="consumerClient">The <see cref="IConsumerClient" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="microservices">The <see cref="IMicroservices" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public SubscriptionsService(
            IConsumerClient consumerClient,
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            IMicroservices microservices,
            ILogger logger)
        {
            _consumerClient = consumerClient;
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _microservices = microservices;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task<SubscriptionResponse> Subscribe(Subscription subscription, ServerCallContext context)
        {
            EventHorizon eventHorizon = null;
            try
            {
                eventHorizon = new EventHorizon(
                    _executionContextManager.Current.BoundedContext.Value,
                    _executionContextManager.Current.Tenant,
                    subscription.Microservice.To<Microservice>(),
                    subscription.Tenant.To<TenantId>());
                var microserviceAddress = _microservices.GetAddressFor(eventHorizon.ProducerMicroservice);
                _consumerClient.AcknowledgeConsent(eventHorizon, microserviceAddress);
                _ = _consumerClient.SubscribeTo(eventHorizon, microserviceAddress);
                return Task.FromResult(new SubscriptionResponse { Success = true });
            }
            catch (Exception)
            {
                _logger.Error($"Error while subscribing to tenant '{eventHorizon.ProducerTenant}' in microservice '{eventHorizon.ProducerMicroservice}'");
                return Task.FromResult(new SubscriptionResponse { Success = false });
            }
        }
    }
}
