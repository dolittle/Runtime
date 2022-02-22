// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.EventHorizon.Producer;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _totalIncomingSubscriptions;
    readonly Counter _totalRejectedSubscriptions;
    readonly Counter _totalAcceptedSubscriptions;
    readonly Counter _totalEventsWrittenToEventHorizon;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalIncomingSubscriptions = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_producer_incoming_subscriptions_total",
            "ConsumerService total number of subscription received from other Runtimes");

        _totalRejectedSubscriptions = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_producer_rejected_subscriptions_total",
            "ConsumerService total number of rejected subscriptions");

        _totalAcceptedSubscriptions = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_producer_accepted_subscriptions_total",
            "ConsumerService total number of accepted subscriptions");
        _totalEventsWrittenToEventHorizon = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_producer_events_written_total",
            "ConsumerService total number of events written to event horizon");
    }

    /// <inheritdoc/>
    public void IncrementTotalIncomingSubscriptions()
        => _totalIncomingSubscriptions.Inc();

    /// <inheritdoc/>
    public void IncrementTotalRejectedSubscriptions()
        => _totalRejectedSubscriptions.Inc();

    /// <inheritdoc/>
    public void IncrementTotalAcceptedSubscriptions()
        => _totalAcceptedSubscriptions.Inc();

    /// <inheritdoc/>
    public void IncrementTotalEventsWrittenToEventHorizon()
        => _totalEventsWrittenToEventHorizon.Inc();
}
