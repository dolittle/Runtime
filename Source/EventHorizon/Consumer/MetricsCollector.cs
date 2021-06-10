// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetricsCollector"/>.
    /// </summary>
    [Singleton]
    public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
    {
        Counter _totalSubscriptionRequests;
        Counter _totalRegisteredSubscriptions;
        Gauge _totalConnectedSubscriptions;


        /// <inheritdoc/>
        public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
        {
            _totalSubscriptionRequests = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_consumer_subscription_requests_total",
                "Subscriptions total number of subscription requests received from Head");

            _totalRegisteredSubscriptions = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_consumer_registered_subscriptions_total",
                "Subscriptions total number of registered subscriptions");

            _totalConnectedSubscriptions = metricFactory.Gauge(
                "dolittle_shared_runtime_event_horizon_consumer_connected_subscriptions",
                "Subscription total number of connected subscriptions");

            return new Collector[]
            {
                _totalSubscriptionRequests,
                _totalRegisteredSubscriptions,
                _totalConnectedSubscriptions
            };
        }

        /// <inheritdoc/>
        public void IncrementTotalSubscriptionsInitiatedFromHead()
            => _totalSubscriptionRequests.Inc();

        /// <inheritdoc/>
        public void IncrementTotalRegisteredSubscriptions()
            => _totalRegisteredSubscriptions.Inc();

        /// <inheritdoc/>
        public void IncrementTotalConnectedSubscriptions()
            => _totalConnectedSubscriptions.Inc();

        public void DecrementTotalConnectedSubscriptions()
            => _totalConnectedSubscriptions.Dec();
    }
}
