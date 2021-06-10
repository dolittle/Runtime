// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetricsCollector"/>.
    /// </summary>
    [Singleton]
    public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
    {
        Counter _totalEventHorizonEventsProcessed;


        /// <inheritdoc/>
        public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
        {
            _totalEventHorizonEventsProcessed = metricFactory.Counter(
                "dolittle_shared_runtime_event_horizon_consumer_processing_event_horizon_events_processed_total",
                "EventProcessor total number of event horizon events processed");


            return new Collector[]
            {
                _totalEventHorizonEventsProcessed,
            };
        }

        /// <inheritdocs/>
        public void IncrementTotalEventHorizonEventsProcessed()
            => _totalEventHorizonEventsProcessed.Inc();
    }
}
