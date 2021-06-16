// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;
using IMetricFactory = Dolittle.Runtime.Metrics.IMetricFactory;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetricsCollector"/>.
    /// </summary>
    [Singleton]
    public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
    {
        Counter _totalInncommingSubscriptions;
        Counter _totalRejectedSubscriptions;
        Counter _totalAcceptedSubscriptions;
        Counter _totalEventsWrittenToEventHorizon;

        /// <inheritdoc/>
        public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
        {
            _totalInncommingSubscriptions = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_producer_incoming_subscriptions_total",
                "ConsumerService total number of subscription received from other Runtimes");

            _totalRejectedSubscriptions = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_producer_rejected_subscriptions_total",
                "ConsumerService total number of rejected subscriptions");

            _totalAcceptedSubscriptions = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_producer_accepted_subscriptions_total",
                "ConsumerService total number of accepted subscriptions");
            _totalEventsWrittenToEventHorizon = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_producer_events_written_total",
                "ConsumerService total number of events written to event horizon");

            return new Collector[]
            {
                _totalInncommingSubscriptions,
                _totalRejectedSubscriptions,
                _totalAcceptedSubscriptions,
                _totalEventsWrittenToEventHorizon
            };
        }

        /// <inheritdoc/>
        public void IncrementTotalIncomingSubscriptions()
            => _totalInncommingSubscriptions.Inc();

        /// <inheritdoc/>
        public void IncrementTotalRejectedSubscriptions()
            => _totalRejectedSubscriptions.Inc();

        /// <inheritdoc/>
        public void IncrementTotalAcceptedSubscriptions()
            => _totalAcceptedSubscriptions.Inc();

        /// <inheritdoc/>
        public void IncrementTotalEventsWrittenToEventHorizon()
            => _totalEventsWrittenToEventHorizon.Inc();
    }
}
