// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Prometheus;

namespace Dolittle.Runtime.Metrics;

/// <summary>
/// Represents an implementation of <see cref="MetricFactory"/>.
/// </summary>
[Singleton]
public class MetricFactory : IMetricFactory
{
    readonly Prometheus.MetricFactory _innerMetricFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricFactory"/> class.
    /// </summary>
    /// <param name="innerMetricFactory">The inner <see cref="Prometheus.MetricFactory"/> for Prometheus.</param>
    public MetricFactory(Prometheus.MetricFactory innerMetricFactory)
    {
        _innerMetricFactory = innerMetricFactory;
    }

    /// <inheritdoc/>
    public Counter Counter(string name, string help, CounterConfiguration configuration = null)
        => _innerMetricFactory.CreateCounter(name, help, configuration);

    /// <inheritdoc/>
    public Counter Counter(string name, string help, params string[] labelNames)
        => _innerMetricFactory.CreateCounter(name, help, labelNames);

    /// <inheritdoc/>
    public Gauge Gauge(string name, string help, GaugeConfiguration configuration = null)
        => _innerMetricFactory.CreateGauge(name, help, configuration);

    /// <inheritdoc/>
    public Gauge Gauge(string name, string help, params string[] labelNames)
        => _innerMetricFactory.CreateGauge(name, help, labelNames);

    /// <inheritdoc/>
    public Histogram Histogram(string name, string help, HistogramConfiguration configuration = null)
        => _innerMetricFactory.CreateHistogram(name, help, configuration);

    /// <inheritdoc/>
    public Histogram Histogram(string name, string help, params string[] labelNames)
        => _innerMetricFactory.CreateHistogram(name, help, labelNames);

    /// <inheritdoc/>
    public Summary Summary(string name, string help, SummaryConfiguration configuration = null)
        => _innerMetricFactory.CreateSummary(name, help, configuration);

    /// <inheritdoc/>
    public Summary Summary(string name, string help, params string[] labelNames)
        => _innerMetricFactory.CreateSummary(name, help, labelNames);
}
