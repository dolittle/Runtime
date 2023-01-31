// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _registrationsTotal;
    readonly Counter _failedRegistrationsTotal;
    readonly Counter _initializationsTotal;
    readonly Counter _startsTotal;
    readonly Counter _failuresTotal;
    readonly Counter _positionSetTotal;
    readonly Counter _initialPositionSetForAllTenantsTotal;
    readonly Histogram _eventProcessingTime;
    readonly Counter _failedEventsProcessedTotal;


    /// <summary>
    /// Creates a new instance of the <see cref="MetricsCollector"/> class.
    /// </summary>
    /// <param name="metricFactory">The metric factory to use to create metrics.</param>
    public MetricsCollector(IMetricFactory metricFactory)
    {
        _registrationsTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_streamprocessors_registrations_total",
            "Total number of stream processors registration attempts per event processor kind",
            new[] { "eventProcessorKind" });

        _failedRegistrationsTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_streamprocessors_failed_registrations_total",
            "Total number of failed stream processor registrations per event processor kind.",
            new[] { "eventProcessorKind" });

        _initializationsTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_streamprocessors_initializations_total",
            "Total number of stream processor initializations per event processor kind.",
            new[] { "eventProcessorKind" });

        _startsTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_streamprocessors_starts_total",
            "Total number of stream processor starts per event processor kind.",
            new[] { "eventProcessorKind" });

        _failuresTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_streamprocessors_failures_total",
            "Total number of stream processor faliures per event processor kind.",
            new[] { "eventProcessorKind" });

        _positionSetTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_streamprocessors_position_set_total",
            "Total number of times a stream processor has been reset to a specific position for one tenant per event processor kind.",
            new[] { "eventProcessorKind" });
            
        _initialPositionSetForAllTenantsTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_streamprocessors_initial_position_set_for_all_tenants_total",
            "Total number of times a stream processor has been reset the the beginning for all tenants per event processor kind.",
            new[] { "eventProcessorKind" });

        _eventProcessingTime = metricFactory.CreateHistogram(
            "dolittle_system_runtime_streamprocessors_event_processing_time_seconds",
            "The time spent processing events per event processor kind.",
            new[] { "eventProcessorKind" },
            new[] { 0.001, 0.01, 0.1, 1, 10 });

        _failedEventsProcessedTotal = metricFactory.CreateCounter(
            "dolittle_system_runtime_streamprocessors_failed_event_processings_total",
            "Total number of event processing attempts per event processor kind.",
            new[] { "eventProcessorKind" });
    }

    /// <inheritdoc />
    public void IncrementRegistrations(EventProcessorKind kind)
        =>  _registrationsTotal.WithLabels(kind).Inc();


    /// <inheritdoc />
    public void IncrementFailedRegistrations(EventProcessorKind kind)
        =>  _failedRegistrationsTotal.WithLabels(kind).Inc();

    /// <inheritdoc />
    public void IncrementInitializations(EventProcessorKind kind)
        =>  _initializationsTotal.WithLabels(kind).Inc();

    /// <inheritdoc />
    public void IncrementStarts(EventProcessorKind kind)
        =>  _startsTotal.WithLabels(kind).Inc();

    /// <inheritdoc />
    public void IncrementFailures(EventProcessorKind kind)
        =>  _failuresTotal.WithLabels(kind).Inc();

    /// <inheritdoc />
    public void IncrementPositionSet(EventProcessorKind kind)
        =>  _positionSetTotal.WithLabels(kind).Inc();

    /// <inheritdoc />
    public void IncrementInitialPositionSetForAllTenants(EventProcessorKind kind)
        =>  _initialPositionSetForAllTenantsTotal.WithLabels(kind).Inc();

    /// <inheritdoc />
    public void IncrementEventsProcessed(EventProcessorKind kind, TimeSpan elapsed)
        =>  _eventProcessingTime.WithLabels(kind).Observe(elapsed.TotalSeconds);

    /// <inheritdoc />
    public void IncrementFailedEventsProcessed(EventProcessorKind kind)
        =>  _failedEventsProcessedTotal.WithLabels(kind).Inc();
}
