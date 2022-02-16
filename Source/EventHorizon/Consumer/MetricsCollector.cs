// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Metrics;
using Prometheus;
using IMetricFactory = Dolittle.Runtime.Metrics.IMetricFactory;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Singleton]
public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
{
    Counter _totalSubscriptionRequests;
    Counter _totalRegisteredSubscriptions;
    Counter _totalSubscriptionRequestsWhereAlreadyStarted;
    Counter _totalSubscritionsWithMissingProducerMicroserviceAddress;
    Counter _totalSubscriptionsFailedDueToException;
    Counter _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompleted;
    Counter _totalSubscriptionLoops;
    Gauge _currentConnectedSubscriptions;


    /// <inheritdoc/>
    public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
    {
        _totalSubscriptionRequests = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_requests_total",
            "SubscriptionsService total number of subscription requests received from Head");

        _totalRegisteredSubscriptions = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_registered_subscriptions_total",
            "Subscriptions total number of registered subscriptions");

        _currentConnectedSubscriptions = metricFactory.Gauge(
            "dolittle_shared_runtime_event_horizon_consumer_connected_subscriptions_current",
            "Subscription total number of connected subscriptions");
        _totalSubscriptionRequestsWhereAlreadyStarted = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_already_started_total",
            "Subscriptions total number of subscription requests made where subscription was already started");

        _totalSubscritionsWithMissingProducerMicroserviceAddress = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_with_missing_producer_microservice_address_total",
            "Subscriptions total number of subscriptions where producer mircoservice address configuration was missing");

        _totalSubscriptionsFailedDueToException = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_failed_due_to_exception_total",
            "Subscriptions total number of subscriptions failed due to an exception");

        _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompleted = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_failed_due_to_receiving_or_writing_events_completed_total",
            "Subscriptions total number of subscriptions failed due to receiving or writing events completed");

        _totalSubscriptionLoops = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_subscription_loops_total",
            "Subscriptions total number of subscriptions loops");

        return new Collector[]
        {
            _totalSubscriptionRequests,
            _totalRegisteredSubscriptions,
            _currentConnectedSubscriptions,
            _totalSubscriptionRequestsWhereAlreadyStarted,
            _totalSubscritionsWithMissingProducerMicroserviceAddress,
            _totalSubscriptionsFailedDueToException,
            _totalSubscriptionsFailedDueToReceivingOrWritingEventsCompleted,
            _totalSubscriptionLoops
        };
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
        => _totalSubscritionsWithMissingProducerMicroserviceAddress.Inc();

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
