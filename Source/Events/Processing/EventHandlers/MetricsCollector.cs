// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
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

    // OpenTelemetry
    readonly Counter<long> _customerRegistrationsTotalOtel;
    readonly Counter<long> _systemRegistrationsTotalOtel;
    readonly Counter<long> _customerFailedRegistrationsTotalOtel;
    readonly Counter<long> _systemFailedRegistrationsTotalOtel;
    readonly Counter<long> _customerEventProcessingFailuresTotalOtel;
    readonly Histogram<double> _customerEventProcessingTimeOtel;

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
    
        // OpenTelemetry
        _customerRegistrationsTotalOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_customer_runtime_eventhandlers_registrations_total",
            "count",
            "Total number of event handler registrations",
            new Dictionary<string, object?>
            {
                { "eventHandlerId", "Event handler id" },
                { "eventHandlerAlias", "Event handler alias" }
            });

        _systemRegistrationsTotalOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_eventhandlers_registrations_total",
            "registrations",
            "Total number of event handler registrations");

        _customerFailedRegistrationsTotalOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_customer_runtime_eventhandlers_failed_registrations_total",
            "errors",
            "Total number of failed event handler registrations");

        _systemFailedRegistrationsTotalOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_eventhandlers_failed_registrations_total",
            "errors",
            "Total number of failed event handler registrations");

        _customerEventProcessingFailuresTotalOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_customer_runtime_eventhandlers_event_processing_failures_total",
            "errors",
            "Total number of failed event processing attempts by an event handler");

        _customerEventProcessingTimeOtel = RuntimeMetrics.Meter.CreateHistogram<double>(
            "dolittle_customer_runtime_eventhandlers_event_processing_time_seconds",
            "seconds",
            "The processing time of events processed by event handlers");
    }

    public void IncrementRegistrationsTotal(EventHandlerInfo info)
    {
        string handlerAlias = info.HasAlias ? info.Alias : string.Empty;
        var handlerId = info.Id.EventHandler.ToString();
        _customerRegistrationsTotal.WithLabels(handlerId, handlerAlias).Inc();
        _systemRegistrationsTotal.Inc();
        _customerRegistrationsTotalOtel.Add(1, new KeyValuePair<string, object?>("eventHandlerId", handlerId), new KeyValuePair<string, object?>("eventHandlerAlias", handlerAlias));
        _systemRegistrationsTotalOtel.Add(1);
    }

    public void IncrementFailedRegistrationsTotal(EventHandlerInfo info)
    {
        string handlerAlias = info.HasAlias ? info.Alias : string.Empty;
        var handlerId = info.Id.EventHandler.ToString();
        _customerFailedRegistrationsTotal.WithLabels(handlerId, handlerAlias).Inc();
        _systemFailedRegistrationsTotal.Inc();
        _customerFailedRegistrationsTotalOtel.Add(1, new KeyValuePair<string, object?>("eventHandlerId", handlerId), new KeyValuePair<string, object?>("eventHandlerAlias", handlerAlias));
        _systemFailedRegistrationsTotalOtel.Add(1);
    }

    public void IncrementEventsProcessedTotal(EventHandlerInfo info, TenantId tenant, StreamEvent @event, TimeSpan processingTime)
    {
        var scopeId = info.Id.Scope.ToString();
        var handlerId = info.Id.EventHandler.ToString();
        string handlerAlias = info.HasAlias ? info.Alias : string.Empty;
        var tenantId = tenant.ToString();
        var eventTypeId = @event.Event.Type.Id.ToString();
        var eventTypeAlias = _eventTypes.GetEventTypeAliasOrEmptyString(@event.Event.Type);
        _customerEventProcessingTime.WithLabels(
                scopeId,
                handlerId,
                handlerAlias,
                tenantId,
                eventTypeId,
                eventTypeAlias)
            .Observe(processingTime.TotalSeconds);
        _customerEventProcessingTimeOtel.Record(processingTime.TotalSeconds,
            new KeyValuePair<string, object?>("scope", scopeId),
            new KeyValuePair<string, object?>("eventHandlerId", handlerId),
            new KeyValuePair<string, object?>("eventHandlerAlias", handlerAlias),
            new KeyValuePair<string, object?>("tenantId", tenantId),
            new KeyValuePair<string, object?>("eventTypeId", eventTypeId),
            new KeyValuePair<string, object?>("eventTypeAlias", eventTypeAlias));
    }

    public void IncrementEventProcessingFailuresTotal(EventHandlerInfo info, TenantId tenant, StreamEvent @event)
    {
        var scopeId = info.Id.Scope.ToString();
        var eventHandlerId = info.Id.EventHandler.ToString();
        string handlerAlias = info.HasAlias ? info.Alias : string.Empty;
        var tenantId = tenant.ToString();
        var eventTypeId = @event.Event.Type.Id.ToString();
        var eventHandlerAlias = _eventTypes.GetEventTypeAliasOrEmptyString(@event.Event.Type);
        _customerEventProcessingFailuresTotal.WithLabels(
                scopeId,
                eventHandlerId,
                handlerAlias,
                tenantId,
                eventTypeId,
                eventHandlerAlias)
            .Inc();
        _customerEventProcessingFailuresTotalOtel.Add(1,
            new KeyValuePair<string, object?>("scope", scopeId),
            new KeyValuePair<string, object?>("eventHandlerId", eventHandlerId),
            new KeyValuePair<string, object?>("eventHandlerAlias", handlerAlias),
            new KeyValuePair<string, object?>("tenantId", tenantId),
            new KeyValuePair<string, object?>("eventTypeId", eventTypeId),
            new KeyValuePair<string, object?>("eventTypeAlias", eventHandlerAlias));
    }
}
