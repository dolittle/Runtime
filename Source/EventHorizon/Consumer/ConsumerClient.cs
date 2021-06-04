// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IConsumerClient" />.
    /// </summary>
    public class ConsumerClient : IConsumerClient
    {
        readonly FactoryFor<ISubscriptions> _getSubscriptions;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        public ConsumerClient(FactoryFor<ISubscriptions> getSubscriptions, IExecutionContextManager executionContextManager, ILogger logger)
        {
            _getSubscriptions = getSubscriptions;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }


        /// <inheritdoc/>
        public async Task<SubscriptionResponse> HandleSubscriptionRequest(SubscriptionId subscriptionId, CancellationToken cancellationToken)
        {
            _executionContextManager.CurrentFor(subscriptionId.ConsumerTenantId);
            var subscriptions = _getSubscriptions();
            return await subscriptions.Subscribe(subscriptionId, cancellationToken).ConfigureAwait(false);
        }
    }
}
