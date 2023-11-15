// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector(IMetricFactory metricFactory) : IMetricsCollector
{
    readonly Counter _totalRequestTime = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_reverse_call_requests_seconds_total",
        "ReverseCallDispatcher total time spent writing request and waiting for response");
    readonly Counter _totalRequests = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_reverse_call_requests_total",
        "ReverseCallDispatcher total requests");
    readonly Counter _totalFailedRequests = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_reverse_call_failed_requests_total",
        "ReverseCallDispatcher total failed requests");

    /// <inheritdoc />
    public void AddToTotalRequestTime(TimeSpan requestTime)
        => _totalRequestTime.Inc(requestTime.TotalSeconds);

    /// <inheritdoc />
    public void AddRequest()
        => _totalRequests.Inc();

    /// <inheritdoc />
    public void AddFailedRequest()
        => _totalFailedRequests.Inc();
}
