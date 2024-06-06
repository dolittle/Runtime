// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Metrics;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _totalConnectionAttempts;
    readonly Counter _totalConnectionsFailed;
    readonly Counter _totalSuccessfulResponses;
    readonly Counter _totalFailureResponses;
    readonly Counter _totalEventHorizonEventsHandled;
    readonly Counter _totalEventHorizonEventsFailedHandling;
    readonly Counter _totalTimeSpentConnecting;
    readonly Counter _totalSubscriptionsWithMissingArguments;
    readonly Counter _totalSubscriptionsWithMissingConsent;

    readonly Counter<long> _totalConnectionAttemptsOtel;
    readonly Counter<long> _totalConnectionsFailedOtel;
    readonly Counter<long> _totalSuccessfulResponsesOtel;
    readonly Counter<long> _totalFailureResponsesOtel;
    readonly Counter<long> _totalEventHorizonEventsHandledOtel;
    readonly Counter<long> _totalEventHorizonEventsFailedHandlingOtel;
    readonly Counter<double> _totalTimeSpentConnectingOtel;
    readonly Counter<long> _totalSubscriptionsWithMissingArgumentsOtel;
    readonly Counter<long> _totalSubscriptionsWithMissingConsentOtel;
    
    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalConnectionAttempts = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_connection_attempts_total",
            "EventHorizonConnection total number of connection attempts");

        _totalConnectionsFailed = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_failed_connections_total",
            "EventHorizonConnection total number of failed connections");

        _totalSuccessfulResponses = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_connections_successful_responses_total",
            "EventHorizonConnection total number of successful connection responses");

        _totalFailureResponses = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_connections_failure_responses_total",
            "EventHorizonConnection total number of failure connection responses");

        _totalEventHorizonEventsHandled = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_handled_events_total",
            "EventHorizonConnection total number of event horizon events handled");

        _totalEventHorizonEventsFailedHandling = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_failed_event_handling_total",
            "EventHorizonConnection total number of event horizon events failed handling");

        _totalTimeSpentConnecting = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_time_spent_connecting_to_event_horizon_total",
            "EventHorizonConnection total time spent successfully connecting to an event horizon");

        _totalSubscriptionsWithMissingArguments = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_subscriptions_with_missing_arguments_total",
            "EventHorizonConnection total number of subscriptions failed due to missing request arguments");

        _totalSubscriptionsWithMissingConsent = metricFactory.CreateCounter(
            "dolittle_shared_runtime_event_horizon_consumer_subscriptions_with_missing_consent_total",
            "EventHorizonConnection total number of subscriptions failed due to missing consent");
        
        
        // OpenTelemetry
        _totalConnectionAttemptsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_connection_attempts_total",
            "connection_attempts",
            "EventHorizonConnection total number of connection attempts");

        _totalConnectionsFailedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_failed_connections_total",
            "errors",
            "EventHorizonConnection total number of failed connections");

        _totalSuccessfulResponsesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_connections_successful_responses_total",
            "responses",
            "EventHorizonConnection total number of successful connection responses");

        _totalFailureResponsesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_connections_failure_responses_total",
            "errors",
            "EventHorizonConnection total number of failure connection responses");

        _totalEventHorizonEventsHandledOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_handled_events_total",
            "events",
            "EventHorizonConnection total number of event horizon events handled");

        _totalEventHorizonEventsFailedHandlingOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_failed_event_handling_total",
            "errors",
            "EventHorizonConnection total number of event horizon events failed handling");

        _totalTimeSpentConnectingOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_shared_runtime_event_horizon_consumer_time_spent_connecting_to_event_horizon_total",
            "seconds",
            "EventHorizonConnection total time spent successfully connecting to an event horizon");

        _totalSubscriptionsWithMissingArgumentsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_subscriptions_with_missing_arguments_total",
            "errors",
            "EventHorizonConnection total number of subscriptions failed due to missing request arguments");

        _totalSubscriptionsWithMissingConsentOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_shared_runtime_event_horizon_consumer_subscriptions_with_missing_consent_total",
            "errors",
            "EventHorizonConnection total number of subscriptions failed due to missing consent");
        
    }

    /// <inheritdoc/>
    public void IncrementTotalConnectionAttempts()
    {
        _totalConnectionAttempts.Inc();
        _totalConnectionAttemptsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalConnectionsFailed()
    {
        _totalConnectionsFailed.Inc();
        _totalConnectionsFailedOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalSuccessfulResponses()
    {
        _totalSuccessfulResponses.Inc();
        _totalSuccessfulResponsesOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalFailureResponses()
    {
        _totalFailureResponses.Inc();
        _totalFailureResponsesOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalEventHorizonEventsHandled()
    {
        _totalEventHorizonEventsHandled.Inc();
        _totalEventHorizonEventsHandledOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalEventHorizonEventsFailedHandling()
    {
        _totalEventHorizonEventsFailedHandling.Inc();
        _totalEventHorizonEventsFailedHandlingOtel.Add(1);
    }

    /// <inheritdoc/>
    public void AddTotalTimeSpentConnecting(TimeSpan elapsed)
    {
        _totalTimeSpentConnecting.Inc(elapsed.TotalSeconds);
        _totalTimeSpentConnectingOtel.Add(elapsed.TotalSeconds);
    }

    /// <inheritdoc/>
    public void IncrementTotalSubcriptionsWithMissingArguments()
    {
        _totalSubscriptionsWithMissingArguments.Inc();
        _totalSubscriptionsWithMissingArgumentsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalSubscriptionsWithMissingConsent()
    {
        _totalSubscriptionsWithMissingConsent.Inc();
        _totalSubscriptionsWithMissingConsentOtel.Add(1);
    }
}
