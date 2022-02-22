// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _totalEventHorizonEventsProcessed;
    readonly Counter _totalEventHorizonEventWritesFailed;
    readonly Counter _totalEventsFetched;
    readonly Counter _totalStreamProcessorsStarted;
    readonly Counter _totalStreamProcessorStartAttempts;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalEventHorizonEventsProcessed = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_events_processed_total",
            "EventProcessor total number of event horizon events processed");

        _totalEventHorizonEventWritesFailed = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_event_writes_failed_total",
            "EventProcessor total number of event horizon event writes failed");

        _totalEventsFetched = metricFactory.CreateCounter(
            "dolittle_system_runtime_event_horizon_consumer_events_fetched_total",
            "EventsFromEventHorizonFetcher total number of event horizon events that has been fetched from stream processors");

        _totalStreamProcessorsStarted = metricFactory.CreateCounter(
            "dolittle_system_runtime_event_horizon_consumer_stream_processors_started_total",
            "StreamProcessor total number of stream processors started");

        _totalStreamProcessorStartAttempts = metricFactory.CreateCounter(
            "dolittle_system_runtime_event_horizon_consumer_stream_processor_start_attempts_total",
            "StreamProcessor total number of stream processors attempted started");
    }

    /// <inheritdocs/>
    public void IncrementTotalEventHorizonEventsProcessed()
        => _totalEventHorizonEventsProcessed.Inc();

    /// <inheritdocs/>
    public void IncrementTotalEventHorizonEventWritesFailed()
        => _totalEventHorizonEventWritesFailed.Inc();

    /// <inheritdocs/>
    public void IncrementTotalEventsFetched()
        => _totalEventsFetched.Inc();

    /// <inheritdocs/>
    public void IncrementTotalStreamProcessorStarted()
        => _totalStreamProcessorsStarted.Inc();

    /// <inheritdocs/>
    public void IncrementTotalStreamProcessorStartAttempts()
        => _totalStreamProcessorStartAttempts.Inc();
}
