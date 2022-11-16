// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Prometheus;

namespace Dolittle.Runtime.Metrics;

/// <summary>
/// Extension methods for <see cref="IMetricFactory"/>.
/// </summary>
public static class MetricsFactoryExtensions
{
    /// <summary>
    /// Creates a counter with labels.
    /// </summary>
    /// <param name="factory">The <see cref="IMetricFactory"/> to use.</param>
    /// <param name="name">The name of the counter.</param>
    /// <param name="help">The help description of the counter.</param>
    /// <param name="labelNames">The label names to use for the counter.</param>
    /// <returns>A <see cref="Counter"/> with labels.</returns>
    public static Counter CreateCounter(this IMetricFactory factory, string name, string help, string[] labelNames)
        => factory.CreateCounter(
            name,
            help,
            new CounterConfiguration
            {
                LabelNames = labelNames,
            });
            
    /// <summary>
    /// Creates a gauge with labels.
    /// </summary>
    /// <param name="factory">The <see cref="IMetricFactory"/> to use.</param>
    /// <param name="name">The name of the gauge.</param>
    /// <param name="help">The help description of the gauge.</param>
    /// <param name="labelNames">The label names to use for the gauge.</param>
    /// <returns>A <see cref="Gauge"/> with labels.</returns>
    public static Gauge CreateGauge(this IMetricFactory factory, string name, string help, string[] labelNames)
        => factory.CreateGauge(
            name,
            help,
            new GaugeConfiguration 
            {
                LabelNames = labelNames,
            });

    /// <summary>
    /// Creates a histogram with labels.
    /// </summary>
    /// <param name="factory">The <see cref="IMetricFactory"/> to use.</param>
    /// <param name="name">The name of the histogram.</param>
    /// <param name="help">The help description of the histogram.</param>
    /// <param name="labelNames">The label names to use for the histogram.</param>
    /// <param name="buckets">The buckets to use for the histogram.</param>
    /// <returns>A <see cref="Histogram"/> with labels.</returns>
    public static Histogram CreateHistogram(this IMetricFactory factory, string name, string help, string[] labelNames, double[] buckets)
        => factory.CreateHistogram(
            name,
            help,
            new HistogramConfiguration
            {
                Buckets = buckets,
                LabelNames = labelNames,
            });
}
