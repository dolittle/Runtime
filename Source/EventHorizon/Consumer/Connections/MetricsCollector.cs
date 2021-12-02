// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;
using IMetricFactory = Dolittle.Runtime.Metrics.IMetricFactory;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Singleton]
public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
{
    Counter _totalConnectionAttempts;
    Counter _totalConnectionsFailed;
    Counter _totalSuccessfulResponses;
    Counter _totalFailureResponses;
    Counter _totalEventHorizonEventsHandled;
    Counter _totalEventHorizonEventsFailedHandling;
    Counter _totalTimeSpentConnecting;
    Counter _totalSubscriptionsWithMissingArguments;
    Counter _totalSubscriptionsWithMissingConsent;


    /// <inheritdoc/>
    public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
    {
        _totalConnectionAttempts = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_connection_attempts_total",
            "EventHorizonConnection total number of connection attempts");

        _totalConnectionsFailed = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_failed_connections_total",
            "EventHorizonConnection total number of failed connections");

        _totalSuccessfulResponses = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_connections_successful_responses_total",
            "EventHorizonConnection total number of successful connection responses");

        _totalFailureResponses = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_connections_failure_responses_total",
            "EventHorizonConnection total number of failure connection responses");

        _totalEventHorizonEventsHandled = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_handled_events_total",
            "EventHorizonConnection total number of event horizon events handled");

        _totalEventHorizonEventsFailedHandling = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_failed_event_handling_total",
            "EventHorizonConnection total number of event horizon events failed handling");

        _totalTimeSpentConnecting = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_time_spent_connecting_to_event_horizon_total",
            "EventHorizonConnection total time spent successfully connecting to an event horizon");

        _totalSubscriptionsWithMissingArguments = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_subscriptions_with_missing_arguments_total",
            "EventHorizonConnection total number of subscriptions failed due to missing request arguments");

        _totalSubscriptionsWithMissingConsent = metricFactory.Counter(
            "dolittle_shared_runtime_event_horizon_consumer_subscriptions_with_missing_consent_total",
            "EventHorizonConnection total number of subscriptions failed due to missing consent");

        return new Collector[]
        {
            _totalConnectionAttempts,
            _totalConnectionsFailed,
            _totalSuccessfulResponses,
            _totalFailureResponses,
            _totalEventHorizonEventsHandled,
            _totalEventHorizonEventsFailedHandling,
            _totalTimeSpentConnecting,
            _totalSubscriptionsWithMissingArguments,
            _totalSubscriptionsWithMissingConsent
        };
    }

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