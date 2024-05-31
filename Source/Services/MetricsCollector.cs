// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Metrics;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _totalRequestTime;
    readonly Counter _totalRequests;
    readonly Counter _totalFailedRequests;

    readonly Counter<double> _totalRequestTimeOtel;
    readonly Counter<long> _totalRequestsOtel;
    readonly Counter<long> _totalFailedRequestsOtel;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalRequests = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reverse_call_requests_total",
            "ReverseCallDispatcher total requests");

        _totalFailedRequests = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reverse_call_failed_requests_total",
            "ReverseCallDispatcher total failed requests");

        _totalRequestTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_reverse_call_requests_seconds_total",
            "ReverseCallDispatcher total time spent writing request and waiting for response");

        _totalRequestsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reverse_call_requests_total",
            "count",
            "ReverseCallDispatcher total requests");

        _totalFailedRequestsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_reverse_call_failed_requests_total",
            "count",
            "ReverseCallDispatcher total failed requests");

        _totalRequestTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_reverse_call_requests_seconds_total",
            "count",
            "ReverseCallDispatcher total time spent writing request and waiting for response");
    }

    /// <inheritdoc />
    public void AddToTotalRequestTime(TimeSpan requestTime)
    {
        _totalRequestTime.Inc(requestTime.TotalSeconds);
        _totalRequestTimeOtel.Add(requestTime.TotalSeconds);
    }

    /// <inheritdoc />
    public void AddRequest()
    {
        _totalRequests.Inc();
        _totalRequestsOtel.Add(1);
    }

    /// <inheritdoc />
    public void AddFailedRequest()
    {
        _totalFailedRequests.Inc();
        _totalFailedRequestsOtel.Add(1);
    }
}
