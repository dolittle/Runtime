// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _customerRegistrationsTotal;
    readonly Counter _systemRegistrationsTotal;
    readonly Counter _failedRegistrationsTotal;
    readonly Counter _eventsProcessedTotal;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _customerRegistrationsTotal = metricFactory.CreateCounter(
            "dolittle_runtime_customer_eventhandlers_registrations_total",
            "Total number of event handler registrations");

        _systemRegistrationsTotal = metricFactory.CreateCounter(
            "dolittle_runtime_system_eventhandlers_registrations_total",
            "Total number of event handler registrations");

        _failedRegistrationsTotal = metricFactory.CreateCounter(
            "dolittle_runtime_customer_eventhandlers_failed_registrations_total",
            "Total number of failed event handler registrations");

        _eventsProcessedTotal = metricFactory.CreateCounter(
            "dolittle_runtime_customer_eventhandlers_events_processed_total",
            "Total number of events processed by an event handler");
    }

    /// <inheritdoc />
    public void IncrementRegistrationsTotal(EventHandlerInfo info) 
    {
        _customerRegistrationsTotal.WithLabels();
        _systemRegistrationsTotal.Inc();
    }

    /// <inheritdoc />
    public void IncrementFailedRegistrationsTotal() 
        => _failedRegistrationsTotal.Inc();
        
    /// <inheritdoc />
    public void IncrementEventsProcessedTotal() 
        => _eventsProcessedTotal.Inc();
}
