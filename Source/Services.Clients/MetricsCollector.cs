// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Metrics;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Diagnostics.OpenTelemetry.Metrics;
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
    
    
    readonly Counter<long> _totalStartedConnectionsOtel;
    readonly UpDownCounter<long> _pendingWritesOtel;
    readonly Counter<double> _totalWriteWaitTimeOtel;
    readonly Counter<double> _totalWriteTimeOtel;
    readonly Counter<long> _totalWritesOtel;
    readonly Counter<long> _totalWriteBytesOtel;
    readonly Counter<double> _totalWaitForConnectResponseTimeOtel;
    readonly Counter<long> _totalCancelledConnectionsOtel;
    readonly Counter<long> _totalReceivedMessagesOtel;
    readonly Counter<long> _totalReceivedBytesOtel;
    readonly Counter<long> _totalPingsReceivedOtel;
    readonly Counter<long> _totalPongsSentOtel;
    readonly Counter<long> _totalReceivedRequestsOtel;
    readonly Counter<long> _totalEmptyMessagesReceivedOtel;
    readonly Counter<long> _totalPingTimeoutsOtel;
    readonly Counter<long> _totalFailedRequestHandlersOtel;
    readonly Counter<long> _totalFailedRequestCallbacksOtel;
    readonly Counter<long> _totalFailedResponseWritesOtel;
    readonly Counter<double> _totalRequestHandlingTimeOtel;
    

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
        
        // OpenTelemetry
        _totalStartedConnectionsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_connections_started_total",
            "count",
            "ReverseCall total number of connections that have been started");

        _pendingWritesOtel = RuntimeMetrics.Meter.CreateUpDownCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_pending_writes",
            "count",
            "ReverseCall current pending stream writes waiting");

        _totalWriteWaitTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_clients_reversecalls_stream_write_wait_seconds_total",
            "seconds",
            "ReverseCall total time spent waiting to write to streams");

        _totalWriteTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_clients_reversecalls_stream_write_seconds_total",
            "seconds",
            "ReverseCall total time spent writing to streams");

        _totalWritesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_stream_writes_total",
            "count",
            "ReverseCall total number of writes to streams");

        _totalWriteBytesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_stream_write_bytes_total",
            "count",
            "ReverseCall total number of bytes written to streams");

        _totalWaitForConnectResponseTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_clients_reversecalls_connect_response_wait_seconds_total",
            "seconds",
            "ReverseCall total time spent waiting for connect response");

        _totalCancelledConnectionsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_connections_cancelled_total",
            "count",
            "ReverseCall total number of connections that have been cancelled");

        _totalReceivedMessagesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_messages_received_total",
            "count",
            "ReverseCall total number of messages that have been received");

        _totalReceivedBytesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_stream_read_bytes_total",
            "count",
            "ReverseCall total number of bytes read from streams");

        _totalPingsReceivedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_pings_received_total",
            "count",
            "ReverseCall total number of pings received");

        _totalPongsSentOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_pongs_sent_total",
            "count",
            "ReverseCall total number of pongs sent");

        _totalReceivedRequestsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_requests_received_total",
            "count",
            "ReverseCall total number of requests that have been received");

        _totalEmptyMessagesReceivedOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_empty_messages_received_total",
            "count",
            "ReverseCall total number of empty messages that have been received");

        _totalPingTimeoutsOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_keepalive_timeouts_total",
            "count",
            "ReverseCall total number of times ping keepalive has timed out");

        _totalFailedRequestHandlersOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_requests_failed_handlers_total",
            "count",
            "ReverseCall total number of failed request handlers");

        _totalFailedRequestCallbacksOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_requests_failed_callbacks_total",
            "count",
            "ReverseCall total number of failed request callbacks");

        _totalFailedResponseWritesOtel = RuntimeMetrics.Meter.CreateCounter<long>(
            "dolittle_system_runtime_services_clients_reversecalls_failed_response_writes_total",
            "count",
            "ReverseCall total number of failed response writes");

        _totalRequestHandlingTimeOtel = RuntimeMetrics.Meter.CreateCounter<double>(
            "dolittle_system_runtime_services_clients_reversecalls_request_handling_seconds_total",
            "seconds",
            "ReverseCall total time spent handling requests");
    }

    /// <inheritdoc/>
    public void IncrementTotalStartedConnections()
    {
        _totalStartedConnections.Inc();
        _totalStartedConnectionsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementPendingWrites()
    {
        _pendingWrites.Dec();
        _pendingWritesOtel.Add(-1);
    }

    /// <inheritdoc/>
    public void DecrementPendingWrites()
    {
        _pendingWrites.Inc();
        _pendingWritesOtel.Add(1);
    }

    /// <inheritdoc/>
    public void AddToTotalWriteWaitTime(TimeSpan waitTime)
    {
        _totalWriteWaitTime.Inc(waitTime.TotalSeconds);
        _totalWriteWaitTimeOtel.Add(waitTime.TotalSeconds);
    }

    /// <inheritdoc/>
    public void AddToTotalWriteTime(TimeSpan writeTime)
    {
        _totalWriteTime.Inc(writeTime.TotalSeconds);
        _totalWriteTimeOtel.Add(writeTime.TotalSeconds);
    }

    /// <inheritdoc/>
    public void IncrementTotalWrites()
    {
        _totalWrites.Inc();
        _totalWritesOtel.Add(1);
    }

    /// <inheritdoc/>
    public void AddToTotalWriteBytes(int messageSize)
    {
        _totalWriteBytes.Inc(messageSize);
        _totalWriteBytesOtel.Add(messageSize);
    }

    /// <inheritdoc/>
    public void AddToTotalWaitForConnectResponseTime(TimeSpan waitTime)
    {
        _totalWaitForConnectResponseTime.Inc(waitTime.TotalSeconds);
        _totalWaitForConnectResponseTimeOtel.Add(waitTime.TotalSeconds);
    }

    /// <inheritdoc/>
    public void IncrementTotalCancelledConnections()
    {
        _totalCancelledConnections.Inc();
        _totalCancelledConnectionsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalReceivedMessages()
    {
        _totalReceivedMessages.Inc();
        _totalReceivedMessagesOtel.Add(1);
    }

    /// <inheritdoc/>
    public void AddToTotalReceivedBytes(int messageSize)
    {
        _totalReceivedBytes.Inc(messageSize);
        _totalReceivedBytesOtel.Add(messageSize);
    }

    /// <inheritdoc/>
    public void IncrementTotalPingsReceived()
    {
        _totalPingsReceived.Inc();
        _totalPingsReceivedOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalPongsSent()
    {
        _totalPongsSent.Inc();
        _totalPongsSentOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalRequestsReceived()
    {
        _totalReceivedRequests.Inc();
        _totalReceivedRequestsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalEmptyMessagesReceived()
    {
        _totalEmptyMessagesReceived.Inc();
        _totalEmptyMessagesReceivedOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalPingTimeouts()
    {
        _totalPingTimeouts.Inc();
        _totalPingTimeoutsOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalFailedRequestHandlers()
    {
        _totalFailedRequestHandlers.Inc();
        _totalFailedRequestHandlersOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalFailedRequestCallbacks()
    {
        _totalFailedRequestCallbacks.Inc();
        _totalFailedRequestCallbacksOtel.Add(1);
    }

    /// <inheritdoc/>
    public void IncrementTotalFailedResponseWrites()
    {
        _totalFailedResponseWrites.Inc();
        _totalFailedResponseWritesOtel.Add(1);
    }

    /// <inheritdoc/>
    public void AddToTotalRequestHandlingTime(TimeSpan handleTime)
    {
        _totalRequestHandlingTime.Inc(handleTime.TotalSeconds);
        _totalRequestHandlingTimeOtel.Add(handleTime.TotalSeconds);
    }
}
