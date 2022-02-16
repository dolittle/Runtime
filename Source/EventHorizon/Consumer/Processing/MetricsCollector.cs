// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Metrics;
using Prometheus;
using IMetricFactory = Dolittle.Runtime.Metrics.IMetricFactory;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Singleton]
public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
{
    Counter _totalEventHorizonEventsProcessed;
    Counter _totalEventHorizonEventWritesFailed;
    Counter _totalEventsFetched;
    Counter _totalStreamProcessorsStarted;
    Counter _totalStreamProcessorStartAttempts;


    /// <inheritdoc/>
    public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
    {
        _totalEventHorizonEventsProcessed = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_events_processed_total",
            "EventProcessor total number of event horizon events processed");

        _totalEventHorizonEventWritesFailed = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_event_writes_failed_total",
            "EventProcessor total number of event horizon event writes failed");

        _totalEventsFetched = metricFactory.Counter(
            "dolittle_system_runtime_event_horizon_consumer_events_fetched_total",
            "EventsFromEventHorizonFetcher total number of event horizon events that has been fetched from stream processors");

        _totalStreamProcessorsStarted = metricFactory.Counter(
            "dolittle_system_runtime_event_horizon_consumer_stream_processors_started_total",
            "StreamProcessor total number of stream processors started");

        _totalStreamProcessorStartAttempts = metricFactory.Counter(
            "dolittle_system_runtime_event_horizon_consumer_stream_processor_start_attempts_total",
            "StreamProcessor total number of stream processors attempted started");

        return new Collector[]
        {
            _totalEventHorizonEventsProcessed,
            _totalEventHorizonEventWritesFailed,
            _totalEventsFetched,
            _totalStreamProcessorsStarted,
            _totalStreamProcessorStartAttempts
        };
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
