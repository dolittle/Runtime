// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetricsCollector"/>.
    /// </summary>
    [Singleton]
    public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
    {
        Counter _totalConnectionAttempts;
        Counter _totalConnectionsFailed;
        Counter _totalSuccessfulResponses;
        Counter _totalFailureResponses;
        Counter _totalEventHorizonEventsHandled;
        Counter _totalEventHorizonEventsFailedHandling;


        /// <inheritdoc/>
        public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
        {
            _totalConnectionAttempts = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_consumer_connection_attempts_total",
                "EventHorizonConnection total number of connection attempts");

            _totalConnectionsFailed = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_consumer_failed_connections_total",
                "EventHorizonConnection total number of failed connections");

            _totalSuccessfulResponses = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_consumer_connections_successful_responses_total",
                "EventHorizonConnection total number of successful connection responses");

            _totalFailureResponses = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_consumer_connections_failure_responses_total",
                "EventHorizonConnection total number of failure connection responses");

            _totalEventHorizonEventsHandled = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_consumer_connections_event_horizon_events_handled_total",
                "EventHorizonConnection total number of event horizon events handled");

            _totalEventHorizonEventsFailedHandling = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_consumer_connections_event_horizon_events_failed_handling_total",
                "EventHorizonConnection total number of event horizon events failed handling");

            return new Collector[]
            {
                _totalConnectionAttempts,
                _totalConnectionsFailed,
                _totalSuccessfulResponses,
                _totalFailureResponses,
                _totalEventHorizonEventsHandled,
                _totalEventHorizonEventsFailedHandling
            };
        }

        /// <inheritdoc/>
        public void IncrementTotalConnectionAttempts()
            => _totalConnectionAttempts.Inc();

        /// <inheritdoc/>
        public void IncrementTotalConnectionsFailed()
            => _totalConnectionsFailed.Inc();

        /// <inheritdoc/>
        public void IncrementTotalSuccessfulResponses()
            => _totalSuccessfulResponses.Inc();

        /// <inheritdoc/>
        public void IncrementTotalFailureResponses()
            => _totalFailureResponses.Inc();

        /// <inheritdoc/>
        public void IncrementTotalEventHorizonEventsHandled()
            => _totalEventHorizonEventsHandled.Inc();

        /// <inheritdoc/>
        public void IncrementTotalEventHorizonEventsFailedHandling()
            => _totalEventHorizonEventsFailedHandling.Inc();
    }
}
