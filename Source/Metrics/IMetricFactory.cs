// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Prometheus;

namespace Dolittle.Runtime.Metrics;

/// <summary>
/// Defines a factory for creating different types of metrics.
/// </summary>
/// <remarks>
/// When using labels, there will a unique instance of the metric for all the values in the labels
/// combined. The label values together with name of the metric constitutes the actual key.
/// It is therefor recommended to be conscious about label usage as this could impact measurement
/// performance.
/// </remarks>
public interface IMetricFactory
{
    /// <summary>
    /// Counters only increase in value and reset to zero when the process restarts.
    /// </summary>
    /// <param name="name">Name of the counter.</param>
    /// <param name="help">Help text displayed for the counter.</param>
    /// <param name="configuration"><see cref="CounterConfiguration"/> for more configuration.</param>
    /// <returns>A new <see cref="Prometheus.Counter"/> that will be tracked.</returns>
    Counter Counter(string name, string help, CounterConfiguration configuration = null);

    /// <summary>
    /// Counters only increase in value and reset to zero when the process restarts.
    /// </summary>
    /// <param name="name">Name of the counter.</param>
    /// <param name="help">Help text displayed for the counter.</param>
    /// <param name="labelNames">Labels you want to associate with the counter.</param>
    /// <returns>A new <see cref="Prometheus.Counter"/> that will be tracked.</returns>
    Counter Counter(string name, string help, params string[] labelNames);

    /// <summary>
    /// Gauges can have any numeric value and change arbitrarily.
    /// </summary>
    /// <param name="name">Name of the gauge.</param>
    /// <param name="help">Help text displayed for the gauge.</param>
    /// <param name="configuration"><see cref="GaugeConfiguration"/> for more configuration.</param>
    /// <returns>A new <see cref="Prometheus.Gauge"/> that will be tracked.</returns>
    Gauge Gauge(string name, string help, GaugeConfiguration configuration = null);

    /// <summary>
    /// Gauges can have any numeric value and change arbitrarily.
    /// </summary>
    /// <param name="name">Name of the gauge.</param>
    /// <param name="help">Help text displayed for the gauge.</param>
    /// <param name="labelNames">Labels you want to associate with the gauge.</param>
    /// <returns>A new <see cref="Prometheus.Gauge"/> that will be tracked.</returns>
    Gauge Gauge(string name, string help, params string[] labelNames);

    /// <summary>
    /// Histograms track the size and number of events in buckets.
    /// </summary>
    /// <param name="name">Name of the histogram.</param>
    /// <param name="help">Help text displayed for the histogram.</param>
    /// <param name="configuration"><see cref="HistogramConfiguration"/> for more configuration.</param>
    /// <returns>A new <see cref="Prometheus.Histogram"/> that will be tracked.</returns>
    Histogram Histogram(string name, string help, HistogramConfiguration configuration = null);

    /// <summary>
    /// Histograms track the size and number of events in buckets.
    /// </summary>
    /// <param name="name">Name of the histogram.</param>
    /// <param name="help">Help text displayed for the histogram.</param>
    /// <param name="labelNames">Labels you want to associate with the histogram.</param>
    /// <returns>A new <see cref="Prometheus.Histogram"/> that will be tracked.</returns>
    Histogram Histogram(string name, string help, params string[] labelNames);

    /// <summary>
    /// Summaries track the trends in events over time (10 minutes by default).
    /// </summary>
    /// <param name="name">Name of the summary.</param>
    /// <param name="help">Help text displayed for the summary.</param>
    /// <param name="configuration"><see cref="SummaryConfiguration"/> for more configuration.</param>
    /// <returns>A new <see cref="Prometheus.Summary"/> that will be tracked.</returns>
    Summary Summary(string name, string help, SummaryConfiguration configuration = null);

    /// <summary>
    /// Summaries track the trends in events over time (10 minutes by default).
    /// </summary>
    /// <param name="name">Name of the summary.</param>
    /// <param name="help">Help text displayed for the summary.</param>
    /// <param name="labelNames">Labels you want to associate with the summary.</param>
    /// <returns>A new <see cref="Prometheus.Summary"/> that will be tracked.</returns>
    Summary Summary(string name, string help, params string[] labelNames);
}