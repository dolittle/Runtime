// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly IEventTypes _eventTypes;
    readonly Counter _customerRegistrationsTotal;
    readonly Counter _systemRegistrationsTotal;
    readonly Counter _customerFailedRegistrationsTotal;
    readonly Counter _systemFailedRegistrationsTotal;
    readonly Counter _customerEventProcessingFailuresTotal;
    readonly Histogram _customerEventProcessingTime;

    /// <summary>
    /// Creates a new instance of the <see cref="MetricsCollector"/> class.
    /// </summary>
    /// <param name="metricFactory">The metric factory to use to create metrics.</param>
    /// <param name="eventTypes">The system to use to resolve event aliases from event types.</param>
    public MetricsCollector(IMetricFactory metricFactory, IEventTypes eventTypes)
    {
        _eventTypes = eventTypes;
        _customerRegistrationsTotal = metricFactory.CreateCounter(
            "dolittle_customer_runtime_eventhandlers_registrations_total",
            "Total number of event handler registrations",
            new[] { "eventHandlerId", "eventHandlerAlias" });

        _systemRegistrationsTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_eventhandlers_registrations_total",
            "Total number of event handler registrations");

        _customerFailedRegistrationsTotal = metricFactory.CreateCounter(
            "dolittle_customer_runtime_eventhandlers_failed_registrations_total",
            "Total number of failed event handler registrations",
            new[] { "eventHandlerId", "eventHandlerAlias" });
        
        _systemFailedRegistrationsTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_eventhandlers_failed_registrations_total",
            "Total number of failed event handler registrations");

        _customerEventProcessingFailuresTotal = metricFactory.CreateCounter(
            "dolittle_customer_runtime_eventhandlers_event_processing_failures_total",
            "Total number of failed event processing attempts by an event handler",
                new[] { "scope", "eventHandlerId", "eventHandlerAlias", "tenantId", "eventTypeId", "eventTypeAlias" });
        
        _customerEventProcessingTime = metricFactory.CreateHistogram(
            "dolittle_customer_runtime_eventhandlers_event_processing_time_seconds",
            "The processing time of events processed by event handlers",
                new[] { "scope", "eventHandlerId", "eventHandlerAlias", "tenantId", "eventTypeId", "eventTypeAlias" },
            new[] { 0.001, 0.01, 0.1, 1, 10 });
    }

    /// <inheritdoc />
    public void IncrementRegistrationsTotal(EventHandlerInfo info)
    {
        _customerRegistrationsTotal.WithLabels(info.Id.EventHandler.ToString(), info.HasAlias ? info.Alias : string.Empty).Inc();
        _systemRegistrationsTotal.Inc();
    }

    /// <inheritdoc />
    public void IncrementFailedRegistrationsTotal(EventHandlerInfo info) 
    {
        _customerFailedRegistrationsTotal.WithLabels(info.Id.EventHandler.ToString(), info.HasAlias ? info.Alias : string.Empty).Inc();
        _systemFailedRegistrationsTotal.Inc();
    }

    /// <inheritdoc />
    public void IncrementEventsProcessedTotal(EventHandlerInfo info, TenantId tenant, StreamEvent @event, TimeSpan processingTime)
    {
        _customerEventProcessingTime.WithLabels(
                info.Id.Scope.ToString(),
                info.Id.EventHandler.ToString(),
                info.HasAlias ? info.Alias : string.Empty,
                tenant.ToString(),
                @event.Event.Type.Id.ToString(),
                _eventTypes.GetEventTypeAliasOrEmptyString(@event.Event.Type))
            .Observe(processingTime.TotalSeconds);
    }

    /// <inheritdoc />
    public void IncrementEventProcessingFailuresTotal(EventHandlerInfo info, TenantId tenant, StreamEvent @event)
        => _customerEventProcessingFailuresTotal.WithLabels(
                info.Id.Scope.ToString(),
                info.Id.EventHandler.ToString(),
                info.HasAlias ? info.Alias : string.Empty,
                tenant.ToString(),
                @event.Event.Type.Id.ToString(),
                _eventTypes.GetEventTypeAliasOrEmptyString(@event.Event.Type))
            .Inc();
}
