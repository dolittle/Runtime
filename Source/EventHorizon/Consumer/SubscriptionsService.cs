// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;
using Grpc.Core;
using static contracts::Dolittle.Runtime.EventHorizon.Subscriptions;
using grpc = contracts::Dolittle.Runtime.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    [Singleton]
    public class SubscriptionsService : SubscriptionsBase
    {
        readonly FactoryFor<IConsumerClient> _getConsumerClient;
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsService"/> class.
        /// </summary>
        /// <param name="getConsumerClient">The <see cref="FactoryFor{T}" /><see cref="IConsumerClient" />.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public SubscriptionsService(
            FactoryFor<IConsumerClient> getConsumerClient,
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            ILogger logger)
        {
            _getConsumerClient = getConsumerClient;
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<grpc.SubscriptionResponse> Subscribe(grpc.Subscription subscriptionRequest, ServerCallContext context)
        {
            var consumerTenant = _executionContextManager.Current.Tenant;
            var subscription = new Subscription(
                consumerTenant,
                subscriptionRequest.Microservice.To<Microservice>(),
                subscriptionRequest.Tenant.To<TenantId>(),
                subscriptionRequest.Scope.To<ScopeId>(),
                subscriptionRequest.Stream.To<StreamId>(),
                subscriptionRequest.Partition.To<PartitionId>());
            var subscriptionResponse = await _getConsumerClient().HandleSubscription(subscription).ConfigureAwait(false);

            return subscriptionResponse.Success ?
                new grpc.SubscriptionResponse()
                : new grpc.SubscriptionResponse { Failure = new grpc.Failure { Reason = subscriptionResponse.FailureReason } };
        }
    }
}
