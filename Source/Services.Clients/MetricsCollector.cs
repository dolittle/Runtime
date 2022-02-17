// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Metrics;
using Prometheus;
using IMetricFactory = Dolittle.Runtime.Metrics.IMetricFactory;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents an implementation of <see cref="IMetricsCollector"/>.
/// </summary>
[Singleton]
public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
{
    Counter _totalStartedConnections;
    Gauge _pendingWrites;
    Counter _totalWriteWaitTime;
    Counter _totalWriteTime;
    Counter _totalWrites;
    Counter _totalWriteBytes;
    Counter _totalWaitForConnectResponseTime;
    Counter _totalCancelledConnections;
    Counter _totalReceivedMessages;
    Counter _totalReceivedBytes;
    Counter _totalPingsReceived;
    Counter _totalPongsSent;
    Counter _totalReceivedRequests;
    Counter _totalEmptyMessagesReceived;
    Counter _totalPingTimeouts;
    Counter _totalFailedRequestHandlers;
    Counter _totalFailedRequestCallbacks;
    Counter _totalFailedResponseWrites;
    Counter _totalRequestHandlingTime;

    /// <inheritdoc/>
    public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
    {
        _totalStartedConnections = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_connections_started_total",
            "ReverseCall total number of connections that have been started");

        _pendingWrites = metricFactory.Gauge(
            "dolittle_system_runtime_services_clients_reversecalls_pending_writes",
            "ReverseCall current pending stream writes waiting");

        _totalWriteWaitTime = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_write_wait_seconds_total",
            "ReverseCall total time spent waiting to write to streams");

        _totalWriteTime = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_write_seconds_total",
            "ReverseCall total time spent writing to streams");

        _totalWrites = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_writes_total",
            "ReverseCall total number of writes to streams");

        _totalWriteBytes = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_write_bytes_total",
            "ReverseCall total number of bytes written to streams");

        _totalWaitForConnectResponseTime = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_connect_response_wait_seconds_total",
            "ReverseCall total time spent waiting for connect response");

        _totalCancelledConnections = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_connections_cancelled_total",
            "ReverseCall total number of connections that have been cancelled");

        _totalReceivedMessages = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_messages_received_total",
            "ReverseCall total number of messages that have been received");

        _totalReceivedBytes = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_stream_read_bytes_total",
            "ReverseCall total number of bytes read from streams");

        _totalPingsReceived = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_pings_received_total",
            "ReverseCall total number of pings received");

        _totalPongsSent = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_pongs_sent_total",
            "ReverseCall total number of pongs sent");

        _totalReceivedRequests = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_requests_received_total",
            "ReverseCall total number of requests that have been received");

        _totalEmptyMessagesReceived = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_empty_messages_received_total",
            "ReverseCall total number of empty messages that have been received");

        _totalPingTimeouts = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_keepalive_timeouts_total",
            "ReverseCall total number of times ping keepalive has timed out");

        _totalFailedRequestHandlers = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_requests_failed_handlers_total",
            "ReverseCall total number of failed request handlers");

        _totalFailedRequestCallbacks = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_requests_failed_callbacks_total",
            "ReverseCall total number of failed request callbacks");

        _totalFailedResponseWrites = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_failed_response_writes_total",
            "ReverseCall total number of failed response writes");

        _totalRequestHandlingTime = metricFactory.Counter(
            "dolittle_system_runtime_services_clients_reversecalls_request_handling_seconds_total",
            "ReverseCall total time spent handling requests");

        return new Collector[]
        {
            _totalStartedConnections,
            _pendingWrites,
            _totalWriteWaitTime,
            _totalWriteTime,
            _totalWrites,
            _totalWriteBytes,
            _totalWaitForConnectResponseTime,
            _totalCancelledConnections,
            _totalReceivedMessages,
            _totalReceivedBytes,
            _totalPingsReceived,
            _totalPongsSent,
            _totalReceivedRequests,
            _totalEmptyMessagesReceived,
            _totalPingTimeouts,
            _totalFailedRequestHandlers,
            _totalFailedRequestCallbacks,
            _totalFailedResponseWrites,
            _totalRequestHandlingTime,
        };
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
