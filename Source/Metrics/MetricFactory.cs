/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Lifecycle;
using Prometheus;

namespace Dolittle.Runtime.Metrics
{
    /// <summary>
    /// Represents an implementation of <see cref="MetricFactory"/>
    /// </summary>
    [Singleton]
    public class MetricFactory : IMetricFactory
    {
        readonly Prometheus.MetricFactory   _innerMetricFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="MetricFactory"/>
        /// </summary>
        /// <param name="innerMetricFactory">The inner <see cref="Prometheus.MetricFactory"/> for Prometheus</param>
        public MetricFactory(Prometheus.MetricFactory innerMetricFactory)
        {
            _innerMetricFactory = innerMetricFactory;
        }

        /// <inheritdoc/>
        public Counter Counter(string name, string help, CounterConfiguration configuration = null)
        {
            return _innerMetricFactory.CreateCounter(name, help, configuration);
        }

        /// <inheritdoc/>
        public Counter Counter(string name, string help, params string[] labelNames)
        {
            return _innerMetricFactory.CreateCounter(name, help, labelNames);
        }

        /// <inheritdoc/>
        public Gauge Gauge(string name, string help, GaugeConfiguration configuration = null)
        {
            return _innerMetricFactory.CreateGauge(name, help, configuration);
        }

        /// <inheritdoc/>
        public Gauge Gauge(string name, string help, params string[] labelNames)
        {
            return _innerMetricFactory.CreateGauge(name, help, labelNames);
        }

        /// <inheritdoc/>
        public Histogram Histogram(string name, string help, HistogramConfiguration configuration = null)
        {
            return _innerMetricFactory.CreateHistogram(name, help, configuration);
        }

        /// <inheritdoc/>
        public Histogram Histogram(string name, string help, params string[] labelNames)
        {
            return _innerMetricFactory.CreateHistogram(name, help, labelNames);
        }

        /// <inheritdoc/>
        public Summary Summary(string name, string help, SummaryConfiguration configuration = null)
        {
            return _innerMetricFactory.CreateSummary(name, help, configuration);
        }

        /// <inheritdoc/>
        public Summary Summary(string name, string help, params string[] labelNames)
        {
            return _innerMetricFactory.CreateSummary(name, help, labelNames);
        }
    }
}