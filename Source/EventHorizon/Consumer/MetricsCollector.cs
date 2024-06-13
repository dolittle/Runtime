// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Metrics;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _totalSubscriptionRequests;
    readonly Counter _totalRegisteredSubscriptions;
    readonly Counter _totalSubscriptionRequestsWhereAlreadyStarted;
    readonly Counter _totalSubscriptionsWithMissingProducerMicroserviceAddress;
    readonly Counter _totalSubscriptionsFailedDueToException;
    readonly Counter _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompleted;
    readonly Counter _totalSubscriptionLoops;
    readonly Gauge _currentConnectedSubscriptions;

    readonly Counter<long> _totalSubscriptionRequestsOtel;
    readonly Counter<long> _totalRegisteredSubscriptionsOtel;
    readonly Counter<long> _totalSubscriptionRequestsWhereAlreadyStartedOtel;
    readonly Counter<long> _totalSubscriptionsWithMissingProducerMicroserviceAddressOtel;
    readonly Counter<long> _totalSubscriptionsFailedDueToExceptionOtel;
    readonly Counter<long> _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompletedOtel;
    readonly Counter<long> _totalSubscriptionLoopsOtel;
    readonly UpDownCounter<long> _currentConnectedSubscriptionsOtel;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalSubscriptionRequests = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_requests_total",
            "SubscriptionsService total number of subscription requests received from Head");

        _totalRegisteredSubscriptions = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_registered_subscriptions_total",
            "Subscriptions total number of registered subscriptions");

        _currentConnectedSubscriptions = metricFactory.CreateGauge(
            "dolittle_shared_runtime_event_horizon_consumer_connected_subscriptions_current",
            "Subscription total number of connected subscriptions");

        _totalSubscriptionRequestsWhereAlreadyStarted = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_already_started_total",
            "Subscriptions total number of subscription requests made where subscription was already started");

        _totalSubscriptionsWithMissingProducerMicroserviceAddress = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_with_missing_producer_microservice_address_total",
            "Subscriptions total number of subscriptions where producer microservice address configuration was missing");

        _totalSubscriptionsFailedDueToException = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_failed_due_to_exception_total",
            "Subscriptions total number of subscriptions failed due to an exception");

        _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompleted = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_failed_due_to_receiving_or_writing_events_completed_total",
            "Subscriptions total number of subscriptions failed due to receiving or writing events completed");

        _totalSubscriptionLoops = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_loops_total",
            "Subscriptions total number of subscriptions loops");
        
        _totalSubscriptionRequestsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_requests_total",
            "requests",
            "SubscriptionsService total number of subscription requests received from Head");

        _totalRegisteredSubscriptionsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_registered_subscriptions_total",
            "subscriptions",
            "Subscriptions total number of registered subscriptions");

        _currentConnectedSubscriptionsOtel = RuntimeMetrics.Meter.CreateUpDownCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_connected_subscriptions_current",
            "subscriptions",
            "Subscription total number of connected subscriptions");

        _totalSubscriptionRequestsWhereAlreadyStartedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_already_started_total",
            "subscriptions",
            "Subscriptions total number of subscription requests made where subscription was already started");

        _totalSubscriptionsWithMissingProducerMicroserviceAddressOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_with_missing_producer_microservice_address_total",
            "errors",
            "Subscriptions total number of subscriptions where producer microservice address configuration was missing");

        _totalSubscriptionsFailedDueToExceptionOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_failed_due_to_exception_total",
            "errors",
            "Subscriptions total number of subscriptions failed due to an exception");

        _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompletedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_failed_due_to_receiving_or_writing_events_completed_total",
            "errors",
            "Subscriptions total number of subscriptions failed due to receiving or writing events completed");

        _totalSubscriptionLoopsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_loops_total",
            "loops",
            "Subscriptions total number of subscriptions loops");
    }

    /// <inheritdoc/>
    public void IncrementTotalSubscriptionsInitiatedFromHead()
    {
        _totalSubscriptionRequests.Inc();
        _totalSubscriptionRequestsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalRegisteredSubscriptions()
    {
        _totalRegisteredSubscriptions.Inc();
        _totalRegisteredSubscriptionsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementCurrentConnectedSubscriptions()
    {
        _currentConnectedSubscriptions.Inc();
        _currentConnectedSubscriptionsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void DecrementCurrentConnectedSubscriptions()
    {
        _currentConnectedSubscriptions.Dec();
        _currentConnectedSubscriptionsOtel.Add(-1);
    }

    /// <inheritdoc/>
    public void IncrementSubscriptionsAlreadyStarted()
    {
        _totalSubscriptionRequestsWhereAlreadyStarted.Inc();
        _totalSubscriptionRequestsWhereAlreadyStartedOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementSubscriptionsMissingProducerMicroserviceAddress()
    {
        _totalSubscriptionsWithMissingProducerMicroserviceAddress.Inc();
        _totalSubscriptionsWithMissingProducerMicroserviceAddressOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementSubscriptionsFailedDueToException()
    {
        _totalSubscriptionsFailedDueToException.Inc();
        _totalSubscriptionsFailedDueToExceptionOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementSubscriptionsFailedDueToReceivingOrWritingEventsCompleted()
    {
        _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompleted.Inc();
        _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompletedOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementSubscriptionLoops()
    {
        _totalSubscriptionLoops.Inc();
        _totalSubscriptionLoopsOtel.Add(1);
    }
}
