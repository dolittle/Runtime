// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion.Lifecycle;
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
    }

    /// <inheritdoc/>
    public void IncrementTotalSubscriptionsInitiatedFromHead()
        => _totalSubscriptionRequests.Inc();

    /// <inheritdoc/>
    public void IncrementTotalRegisteredSubscriptions()
        => _totalRegisteredSubscriptions.Inc();

    /// <inheritdoc/>
    public void IncrementCurrentConnectedSubscriptions()
        => _currentConnectedSubscriptions.Inc();

    /// <inheritdoc/>
    public void DecrementCurrentConnectedSubscriptions()
        => _currentConnectedSubscriptions.Dec();

    /// <inheritdoc/>
    public void IncrementSubscriptionsAlreadyStarted()
        => _totalSubscriptionRequestsWhereAlreadyStarted.Inc();

    /// <inheritdoc/>
    public void IncrementSubscriptionsMissingProducerMicroserviceAddress()
        => _totalSubscriptionsWithMissingProducerMicroserviceAddress.Inc();

    /// <inheritdoc/>
    public void IncrementSubscriptionsFailedDueToException()
        => _totalSubscriptionsFailedDueToException.Inc();

    /// <inheritdoc/>
    public void IncrementSubscriptionsFailedDueToReceivingOrWritingEventsCompleted()
        => _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompleted.Inc();

    /// <inheritdoc/>
    public void IncrementSubscriptionLoops()
        => _totalSubscriptionLoops.Inc();
}
