// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector(IMetricFactory metricFactory) : IMetricsCollector
{
    readonly Counter _totalConnectionAttempts = metricFactory.CreateCounter(
        "dolittle_shared_runtime_event_horizon_consumer_connection_attempts_total",
        "EventHorizonConnection total number of connection attempts");
    readonly Counter _totalConnectionsFailed = metricFactory.CreateCounter(
        "dolittle_shared_runtime_event_horizon_consumer_failed_connections_total",
        "EventHorizonConnection total number of failed connections");
    readonly Counter _totalSuccessfulResponses = metricFactory.CreateCounter(
        "dolittle_shared_runtime_event_horizon_consumer_connections_successful_responses_total",
        "EventHorizonConnection total number of successful connection responses");
    readonly Counter _totalFailureResponses = metricFactory.CreateCounter(
        "dolittle_shared_runtime_event_horizon_consumer_connections_failure_responses_total",
        "EventHorizonConnection total number of failure connection responses");
    readonly Counter _totalEventHorizonEventsHandled = metricFactory.CreateCounter(
        "dolittle_shared_runtime_event_horizon_consumer_handled_events_total",
        "EventHorizonConnection total number of event horizon events handled");
    readonly Counter _totalEventHorizonEventsFailedHandling = metricFactory.CreateCounter(
        "dolittle_shared_runtime_event_horizon_consumer_failed_event_handling_total",
        "EventHorizonConnection total number of event horizon events failed handling");
    readonly Counter _totalTimeSpentConnecting = metricFactory.CreateCounter(
        "dolittle_shared_runtime_event_horizon_consumer_time_spent_connecting_to_event_horizon_total",
        "EventHorizonConnection total time spent successfully connecting to an event horizon");
    readonly Counter _totalSubscriptionsWithMissingArguments = metricFactory.CreateCounter(
        "dolittle_shared_runtime_event_horizon_consumer_subscriptions_with_missing_arguments_total",
        "EventHorizonConnection total number of subscriptions failed due to missing request arguments");
    readonly Counter _totalSubscriptionsWithMissingConsent = metricFactory.CreateCounter(
        "dolittle_shared_runtime_event_horizon_consumer_subscriptions_with_missing_consent_total",
        "EventHorizonConnection total number of subscriptions failed due to missing consent");

    /// <inheritdoc/>
    public void IncrementTotalConnectionAttempts()
        => _totalConnectionAttempts.Inc();

    /// <inheritdoc/>
    public void IncrementTotalConnectionsFailed()
        => _totalConnectionsFailed.Inc();

    /// <inheritdoc/>
    public void IncrementTotalSuccessfulResponses()
        => _totalSuccessfulResponses.Inc();

    /// <inheritdoc/>
    public void IncrementTotalFailureResponses()
        => _totalFailureResponses.Inc();

    /// <inheritdoc/>
    public void IncrementTotalEventHorizonEventsHandled()
        => _totalEventHorizonEventsHandled.Inc();

    /// <inheritdoc/>
    public void IncrementTotalEventHorizonEventsFailedHandling()
        => _totalEventHorizonEventsFailedHandling.Inc();

    /// <inheritdoc/>
    public void AddTotalTimeSpentConnecting(TimeSpan elapsed)
        => _totalTimeSpentConnecting.Inc(elapsed.TotalSeconds);

    /// <inheritdoc/>
    public void IncrementTotalSubcriptionsWithMissingArguments()
        => _totalSubscriptionsWithMissingArguments.Inc();

    /// <inheritdoc/>
    public void IncrementTotalSubscriptionsWithMissingConsent()
        => _totalSubscriptionsWithMissingConsent.Inc();
}
