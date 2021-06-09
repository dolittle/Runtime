// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Services.Callbacks
{
    /// <summary>
    /// Represents an implementatino of <see cref="IMetricsCollector"/>.
    /// </summary>
    [Singleton]
    public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
    {
        Counter _totalCallbackTime;
        Counter _totalSchedulesMissed;
        Counter _totalSchedulesMissedTime;
        Counter _totalCallbackLoopsFailed;
        Counter _totalCallbacksCalled;
        Counter _totalCallbacksFailed;
        Counter _totalCallbacksRegistered;
        Counter _totalCallbacksUnregistered;


        /// <inheritdoc/>
        public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
        {
            _totalCallbackTime = metricFactory.Counter(
                "dolittle_system_runtime_services_callbacks_time_seconds_total",
                "Callbacks total time spent calling callbacks");

            _totalSchedulesMissed = metricFactory.Counter(
                "dolittle_system_runtime_services_callbacks_schedules_missed_total",
                "Callbacks total number of missed callback schedules");

            _totalSchedulesMissedTime = metricFactory.Counter(
                "dolittle_system_runtime_services_callbacks_schedules_missed_time_seconds_total",
                "Callbacks total time delays for missed callback schedules");

            _totalCallbacksCalled = metricFactory.Counter(
                "dolittle_system_runtime_services_callbacks_calls_total",
                "Callbacks total number of called callbacks");

            _totalCallbacksFailed = metricFactory.Counter(
                "dolittle_system_runtime_services_callbacks_failed_calls_total",
                "Callbacks total number of called callbacks that failed");

            _totalCallbackLoopsFailed = metricFactory.Counter(
                "dolittle_system_runtime_services_callbacks_failed_call_loops_total",
                "Callbacks total number of failed callback loops");

            _totalCallbacksRegistered = metricFactory.Counter(
                "dolittle_system_runtime_services_callbacks_registered_total",
                "Callbacks total number of registered callbacks");

            _totalCallbacksUnregistered = metricFactory.Counter(
                "dolittle_system_runtime_services_callbacks_unregistered_total",
                "Callbacks total number of unregistered callbacks");

            return new Collector[]
            {
                _totalCallbackTime,
                _totalSchedulesMissed,
                _totalSchedulesMissedTime,
                _totalCallbacksCalled,
                _totalCallbacksFailed,
                _totalCallbackLoopsFailed,
                _totalCallbacksRegistered,
                _totalCallbacksUnregistered,
            };
        }

        /// <inheritdoc/>
        public void AddToTotalCallbackTime(TimeSpan elapsed)
            => _totalCallbackTime.Inc(elapsed.TotalSeconds);

        /// <inheritdoc/>
        public void IncrementTotalSchedulesMissed()
            => _totalSchedulesMissed.Inc();

        /// <inheritdoc/>
        public void AddToTotalSchedulesMissedTime(TimeSpan elapsed)
            => _totalSchedulesMissedTime.Inc(elapsed.TotalSeconds);

        /// <inheritdoc/>
        public void IncrementTotalCallbackLoopsFailed()
            => _totalCallbackLoopsFailed.Inc();

        /// <inheritdoc/>
        public void IncrementTotalCallbacksCalled()
            => _totalCallbacksCalled.Inc();

        /// <inheritdoc/>
        public void IncrementTotalCallbacksFailed()
            => _totalCallbacksFailed.Inc();

        /// <inheritdoc/>
        public void IncrementTotalCallbacksRegistered()
            => _totalCallbacksRegistered.Inc();

        /// <inheritdoc/>
        public void IncrementTotalCallbacksUnregistered()
            => _totalCallbacksUnregistered.Inc();
    }
}
