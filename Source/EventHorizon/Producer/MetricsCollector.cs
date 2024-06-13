// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Metrics;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
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
    
    readonly Counter<long> _totalIncomingSubscriptionsOtel;
    readonly Counter<long> _totalRejectedSubscriptionsOtel;
    readonly Counter<long> _totalAcceptedSubscriptionsOtel;
    readonly Counter<long> _totalEventsWrittenToEventHorizonOtel;

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
        
        _totalIncomingSubscriptionsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_producer_incoming_subscriptions_total",
            "subscriptions",
            "ConsumerService total number of subscription received from other Runtimes");

        _totalRejectedSubscriptionsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_producer_rejected_subscriptions_total",
            "subscriptions",
            "ConsumerService total number of rejected subscriptions");

        _totalAcceptedSubscriptionsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_producer_accepted_subscriptions_total",
            "subscriptions",
            "ConsumerService total number of accepted subscriptions");
        
        _totalEventsWrittenToEventHorizonOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_producer_events_written_total",
            "events",
            "ConsumerService total number of events written to event horizon");
    }

    /// <inheritdoc/>
    public void IncrementTotalIncomingSubscriptions()
    {
        _totalIncomingSubscriptions.Inc();
        _totalIncomingSubscriptionsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalRejectedSubscriptions()
    {
        _totalRejectedSubscriptions.Inc();
        _totalRejectedSubscriptionsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalAcceptedSubscriptions()
    {
        _totalAcceptedSubscriptions.Inc();
        _totalAcceptedSubscriptionsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalEventsWrittenToEventHorizon()
    {
        _totalEventsWrittenToEventHorizon.Inc();
        _totalEventsWrittenToEventHorizonOtel.Add(1);
    }
}
