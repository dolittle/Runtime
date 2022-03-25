// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Metrics, Singleton]
public class MetricsCollector : IMetricsCollector
{
    readonly Counter _totalStartedConnections;
    readonly Gauge _pendingWrites;
    readonly Counter _totalWriteWaitTime;
    readonly Counter _totalWriteTime;
    readonly Counter _totalWrites;
    readonly Counter _totalWriteBytes;
    readonly Counter _totalWaitForConnectResponseTime;
    readonly Counter _totalCancelledConnections;
    readonly Counter _totalReceivedMessages;
    readonly Counter _totalReceivedBytes;
    readonly Counter _totalPingsReceived;
    readonly Counter _totalPongsSent;
    readonly Counter _totalReceivedRequests;
    readonly Counter _totalEmptyMessagesReceived;
    readonly Counter _totalPingTimeouts;
    readonly Counter _totalFailedRequestHandlers;
    readonly Counter _totalFailedRequestCallbacks;
    readonly Counter _totalFailedResponseWrites;
    readonly Counter _totalRequestHandlingTime;

    public MetricsCollector(IMetricFactory metricFactory)
    {
        _totalStartedConnections = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_connections_started_total",
            "ReverseCall total number of connections that have been started");

        _pendingWrites = metricFactory.CreateGauge(
            "dolittle_system_runtime_services_clients_reversecalls_pending_writes",
            "ReverseCall current pending stream writes waiting");

        _totalWriteWaitTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_write_wait_seconds_total",
            "ReverseCall total time spent waiting to write to streams");

        _totalWriteTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_write_seconds_total",
            "ReverseCall total time spent writing to streams");

        _totalWrites = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_writes_total",
            "ReverseCall total number of writes to streams");

        _totalWriteBytes = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_write_bytes_total",
            "ReverseCall total number of bytes written to streams");

        _totalWaitForConnectResponseTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_connect_response_wait_seconds_total",
            "ReverseCall total time spent waiting for connect response");

        _totalCancelledConnections = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_connections_cancelled_total",
            "ReverseCall total number of connections that have been cancelled");

        _totalReceivedMessages = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_messages_received_total",
            "ReverseCall total number of messages that have been received");

        _totalReceivedBytes = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_read_bytes_total",
            "ReverseCall total number of bytes read from streams");

        _totalPingsReceived = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_pings_received_total",
            "ReverseCall total number of pings received");

        _totalPongsSent = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_pongs_sent_total",
            "ReverseCall total number of pongs sent");

        _totalReceivedRequests = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_requests_received_total",
            "ReverseCall total number of requests that have been received");

        _totalEmptyMessagesReceived = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_empty_messages_received_total",
            "ReverseCall total number of empty messages that have been received");

        _totalPingTimeouts = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_keepalive_timeouts_total",
            "ReverseCall total number of times ping keepalive has timed out");

        _totalFailedRequestHandlers = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_requests_failed_handlers_total",
            "ReverseCall total number of failed request handlers");

        _totalFailedRequestCallbacks = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_requests_failed_callbacks_total",
            "ReverseCall total number of failed request callbacks");

        _totalFailedResponseWrites = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_failed_response_writes_total",
            "ReverseCall total number of failed response writes");

        _totalRequestHandlingTime = metricFactory.CreateCounter(
            "dolittle_system_runtime_services_clients_reversecalls_request_handling_seconds_total",
            "ReverseCall total time spent handling requests");
    }

    /// <inheritdoc/>
    public void IncrementTotalStartedConnections()
        => _totalStartedConnections.Inc();

    /// <inheritdoc/>
    public void IncrementPendingWrites()
        => _pendingWrites.Dec();

    /// <inheritdoc/>
    public void DecrementPendingWrites()
        => _pendingWrites.Inc();

    /// <inheritdoc/>
    public void AddToTotalWriteWaitTime(TimeSpan waitTime)
        => _totalWriteWaitTime.Inc(waitTime.TotalSeconds);

    /// <inheritdoc/>
    public void AddToTotalWriteTime(TimeSpan writeTime)
        => _totalWriteTime.Inc(writeTime.TotalSeconds);

    /// <inheritdoc/>
    public void IncrementTotalWrites()
        => _totalWrites.Inc();

    /// <inheritdoc/>
    public void AddToTotalWriteBytes(int messageSize)
        => _totalWriteBytes.Inc(messageSize);

    /// <inheritdoc/>
    public void AddToTotalWaitForConnectResponseTime(TimeSpan waitTime)
        => _totalWaitForConnectResponseTime.Inc(waitTime.TotalSeconds);

    /// <inheritdoc/>
    public void IncrementTotalCancelledConnections()
        => _totalCancelledConnections.Inc();

    /// <inheritdoc/>
    public void IncrementTotalReceivedMessages()
        => _totalReceivedMessages.Inc();

    /// <inheritdoc/>
    public void AddToTotalReceivedBytes(int messageSize)
        => _totalReceivedBytes.Inc(messageSize);

    /// <inheritdoc/>
    public void IncrementTotalPingsReceived()
        => _totalPingsReceived.Inc();

    /// <inheritdoc/>
    public void IncrementTotalPongsSent()
        => _totalPongsSent.Inc();

    /// <inheritdoc/>
    public void IncrementTotalRequestsReceived()
        => _totalReceivedRequests.Inc();

    /// <inheritdoc/>
    public void IncrementTotalEmptyMessagesReceived()
        => _totalEmptyMessagesReceived.Inc();

    /// <inheritdoc/>
    public void IncrementTotalPingTimeouts()
        => _totalPingTimeouts.Inc();

    /// <inheritdoc/>
    public void IncrementTotalFailedRequestHandlers()
        => _totalFailedRequestHandlers.Inc();

    /// <inheritdoc/>
    public void IncrementTotalFailedRequestCallbacks()
        => _totalFailedRequestCallbacks.Inc();

    /// <inheritdoc/>
    public void IncrementTotalFailedResponseWrites()
        => _totalFailedResponseWrites.Inc();

    /// <inheritdoc/>
    public void AddToTotalRequestHandlingTime(TimeSpan handleTime)
        => _totalRequestHandlingTime.Inc(handleTime.TotalSeconds);
}
