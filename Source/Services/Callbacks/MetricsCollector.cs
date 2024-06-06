// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Metrics;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Services.Callbacks;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _totalCallbacksRegistered;
    readonly Counter _totalCallbacksCalled;
    readonly Counter _totalCallbackTime;
    readonly Counter _totalCallbacksFailed;
    readonly Counter _totalCallbacksUnregistered;
    readonly Counter _totalSchedulesMissed;
    readonly Counter _totalSchedulesMissedTime;
    readonly Counter _totalCallbackLoopsFailed;
    
    readonly Counter<long> _totalCallbacksRegisteredOtel;
    readonly Counter<long> _totalCallbacksCalledOtel;
    readonly Counter<double> _totalCallbackTimeOtel;
    readonly Counter<long> _totalCallbacksFailedOtel;
    readonly Counter<long> _totalCallbacksUnregisteredOtel;
    readonly Counter<long> _totalSchedulesMissedOtel;
    readonly Counter<double> _totalSchedulesMissedTimeOtel;
    readonly Counter<long> _totalCallbackLoopsFailedOtel;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalCallbacksRegistered = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_callbacks_registered_total",
            "Callbacks total number of registered callbacks");
        

        _totalCallbacksCalled = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_callbacks_calls_total",
            "Callbacks total number of called callbacks");

        _totalCallbackTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_callbacks_time_seconds_total",
            "Callbacks total time spent calling callbacks");

        _totalCallbacksFailed = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_callbacks_failed_calls_total",
            "Callbacks total number of called callbacks that failed");

        _totalCallbacksUnregistered = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_callbacks_unregistered_total",
            "Callbacks total number of unregistered callbacks");

        _totalSchedulesMissed = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_callbacks_schedules_missed_total",
            "Callbacks total number of missed callback schedules");

        _totalSchedulesMissedTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_callbacks_schedules_missed_time_seconds_total",
            "Callbacks total time delays for missed callback schedules");

        _totalCallbackLoopsFailed = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_callbacks_failed_call_loops_total",
            "Callbacks total number of failed callback loops");
        
        
        // OpenTelemetry
        _totalCallbacksRegisteredOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_callbacks_registered_total",
            "count",
            "Callbacks total number of registered callbacks");

        _totalCallbacksCalledOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_callbacks_calls_total",
            "count",
            "Callbacks total number of called callbacks");

        _totalCallbackTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_callbacks_time_seconds_total",
            "seconds",
            "Callbacks total time spent calling callbacks");

        _totalCallbacksFailedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_callbacks_failed_calls_total",
            "errors",
            "Callbacks total number of called callbacks that failed");

        _totalCallbacksUnregisteredOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_callbacks_unregistered_total",
            "count",
            "Callbacks total number of unregistered callbacks");

        _totalSchedulesMissedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_callbacks_schedules_missed_total",
            "errors",
            "Callbacks total number of missed callback schedules");

        _totalSchedulesMissedTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_callbacks_schedules_missed_time_seconds_total",
            "seconds",
            "Callbacks total time delays for missed callback schedules");

        _totalCallbackLoopsFailedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_callbacks_failed_call_loops_total",
            "errors",
            "Callbacks total number of failed callback loops");
    }

    /// <inheritdoc/>
    public void IncrementTotalCallbacksRegistered()
    {
        _totalCallbacksRegistered.Inc();
        _totalCallbacksRegisteredOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalCallbacksCalled()
    {
        _totalCallbacksCalled.Inc();
        _totalCallbacksCalledOtel.Add(1);
    }

    /// <inheritdoc/>
    public void AddToTotalCallbackTime(TimeSpan elapsed)
    {
        _totalCallbackTime.Inc(elapsed.TotalSeconds);
        _totalCallbackTimeOtel.Add(elapsed.TotalSeconds);
    }

    /// <inheritdoc/>
    public void IncrementTotalCallbacksFailed()
    {
        _totalCallbacksFailed.Inc();
        _totalCallbacksFailedOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalCallbacksUnregistered()
    {
        _totalCallbacksUnregistered.Inc();
        _totalCallbacksUnregisteredOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalSchedulesMissed()
    {
        _totalSchedulesMissed.Inc();
        _totalSchedulesMissedOtel.Add(1);
    }

    /// <inheritdoc/>
    public void AddToTotalSchedulesMissedTime(TimeSpan elapsed)
    {
        _totalSchedulesMissedTime.Inc(elapsed.TotalSeconds);
        _totalSchedulesMissedTimeOtel.Add(elapsed.TotalSeconds);
    }

    /// <inheritdoc/>
    public void IncrementTotalCallbackLoopsFailed()
    {
        _totalCallbackLoopsFailed.Inc();
        _totalCallbackLoopsFailedOtel.Add(1);
    }
}
