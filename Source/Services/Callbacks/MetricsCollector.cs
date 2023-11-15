// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Services.Callbacks;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector(IMetricFactory metricFactory) : IMetricsCollector
{
    readonly Counter _totalCallbacksRegistered = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_callbacks_registered_total",
        "Callbacks total number of registered callbacks");
    readonly Counter _totalCallbacksCalled = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_callbacks_calls_total",
        "Callbacks total number of called callbacks");
    readonly Counter _totalCallbackTime = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_callbacks_time_seconds_total",
        "Callbacks total time spent calling callbacks");
    readonly Counter _totalCallbacksFailed = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_callbacks_failed_calls_total",
        "Callbacks total number of called callbacks that failed");
    readonly Counter _totalCallbacksUnregistered = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_callbacks_unregistered_total",
        "Callbacks total number of unregistered callbacks");
    readonly Counter _totalSchedulesMissed = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_callbacks_schedules_missed_total",
        "Callbacks total number of missed callback schedules");
    readonly Counter _totalSchedulesMissedTime = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_callbacks_schedules_missed_time_seconds_total",
        "Callbacks total time delays for missed callback schedules");
    readonly Counter _totalCallbackLoopsFailed = metricFactory.CreateCounter(
        "dolittle_system_runtime_services_callbacks_failed_call_loops_total",
        "Callbacks total number of failed callback loops");

    /// <inheritdoc/>
    public void IncrementTotalCallbacksRegistered()
        => _totalCallbacksRegistered.Inc();

    /// <inheritdoc/>
    public void IncrementTotalCallbacksCalled()
        => _totalCallbacksCalled.Inc();

    /// <inheritdoc/>
    public void AddToTotalCallbackTime(TimeSpan elapsed)
        => _totalCallbackTime.Inc(elapsed.TotalSeconds);

    /// <inheritdoc/>
    public void IncrementTotalCallbacksFailed()
        => _totalCallbacksFailed.Inc();

    /// <inheritdoc/>
    public void IncrementTotalCallbacksUnregistered()
        => _totalCallbacksUnregistered.Inc();

    /// <inheritdoc/>
    public void IncrementTotalSchedulesMissed()
        => _totalSchedulesMissed.Inc();

    /// <inheritdoc/>
    public void AddToTotalSchedulesMissedTime(TimeSpan elapsed)
        => _totalSchedulesMissedTime.Inc(elapsed.TotalSeconds);

    /// <inheritdoc/>
    public void IncrementTotalCallbackLoopsFailed()
        => _totalCallbackLoopsFailed.Inc();
}
