// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Metrics;
using Prometheus;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetricsCollector"/>.
    /// </summary>
    public class MetricsCollector : ICanProvideMetrics, IMetricsCollector
    {
        Counter _totalReceivedBytes;
        Counter _totalRequestHandlingTime;
        Counter _totalWaitForConnectResponseTime;
        Counter _totalWriteBytes;
        Counter _totalWriteTime;
        Counter _totalWriteWaitTime;
        Gauge _pendingWrites;
        Counter _totalCancelledConnections;
        Counter _totalEmptyMessagesReceived;
        Counter _totalFailedRequestCallbacks;
        Counter _totalFailedRequestHandlers;
        Counter _totalFailedResponseWrites;
        Counter _totalPingsReceived;
        Counter _totalPingTimeouts;
        Counter _totalPongsSent;
        Counter _totalReceivedMessages;
        Counter _totalReceivedRequests;
        Counter _totalStartedConnections;
        Counter _totalWrites;

        /// <inheritdoc/>
        public IEnumerable<Collector> Provide(IMetricFactory metricFactory)
        {
            _totalReceivedBytes = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_stream_read_bytes_total",
                "ReverseCall total number of bytes read from streams");

            _totalRequestHandlingTime = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_request_handling_seconds_total",
                "ReverseCall total time spent handling requests");

            _totalWaitForConnectResponseTime = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_connect_response_wait_seconds_total",
                "ReverseCall total time spent waiting for connect response");

            _totalWriteBytes = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_stream_write_bytes_total",
                "ReverseCall total number of bytes written to streams");

            _totalWriteTime = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_stream_write_seconds_total",
                "ReverseCall total time spent writing to streams");

            _totalWriteWaitTime = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_stream_write_wait_seconds_total",
                "ReverseCall total time spent waiting to write to streams");

            _pendingWrites = metricFactory.Gauge(
                "dolittle_system_runtime_services_clients_reversecalls_pending_writes",
                "ReverseCall current pending stream writes waiting");

            _totalCancelledConnections = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_connections_cancelled_total",
                "ReverseCall total number of connections that have been cancelled");

            _totalEmptyMessagesReceived = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_empty_messages_received_total",
                "ReverseCall total number of empty messages that have been received");

            _totalFailedRequestCallbacks = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_requests_failed_callbacks_total",
                "ReverseCall total number of failed request callbacks");

            _totalFailedRequestHandlers = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_requests_failed_handlers_total",
                "ReverseCall total number of failed request handlers");

            _totalFailedResponseWrites = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_failed_response_writes_total",
                "ReverseCall total number of failed response writes");

            _totalPingsReceived = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_pings_received_total",
                "ReverseCall total number of pings received");

            _totalReceivedBytes = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_stream_read_bytes_total",
                "ReverseCall total number of bytes read from streams");

            _totalPingTimeouts = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_keepalive_timeouts_total",
                "ReverseCall total number of times ping keepalive has timed out");

            _totalPongsSent = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_pongs_sent_total",
                "ReverseCall total number of pongs sent");

            _totalReceivedMessages = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_messages_received_total",
                "ReverseCall total number of messages that have been received");

            _totalReceivedRequests = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_requests_received_total",
                "ReverseCall total number of requests that have been received");

            _totalStartedConnections = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_connections_started_total",
                "ReverseCall total number of connections that have been started");

            _totalWrites = metricFactory.Counter(
                "dolittle_system_runtime_services_clients_reversecalls_stream_writes_total",
                "ReverseCall total number of writes to streams");

            return new Collector[]
            {
                _totalReceivedBytes,
                _totalRequestHandlingTime,
                _totalWaitForConnectResponseTime,
                _totalWriteBytes,
                _totalWriteTime,
                _totalWriteWaitTime,
                _pendingWrites,
                _totalCancelledConnections,
                _totalEmptyMessagesReceived,
                _totalFailedRequestCallbacks,
                _totalFailedRequestHandlers,
                _totalFailedResponseWrites,
                _totalPingsReceived,
                _totalReceivedBytes,
                _totalPingTimeouts,
                _totalPongsSent,
                _totalReceivedMessages,
                _totalReceivedRequests,
                _totalStartedConnections,
                _totalWrites,
            };
        }

        /// <inheritdoc/>
        public void AddToTotalReceivedBytes(int messageSize)
            => _totalReceivedBytes.Inc(messageSize);

        /// <inheritdoc/>
        public void AddToTotalRequestHandlingTime(TimeSpan handleTime)
            => _totalRequestHandlingTime.Inc(handleTime.TotalSeconds);

        /// <inheritdoc/>
        public void AddToTotalWaitForConnectResponseTime(TimeSpan waitTime)
            => _totalWaitForConnectResponseTime.Inc(waitTime.TotalSeconds);

        /// <inheritdoc/>
        public void AddToTotalWriteBytes(int messageSize)
            => _totalWriteBytes.Inc(messageSize);

        /// <inheritdoc/>
        public void AddToTotalWriteTime(TimeSpan writeTime)
            => _totalWriteTime.Inc(writeTime.TotalSeconds);

        /// <inheritdoc/>
        public void AddToTotalWriteWaitTime(TimeSpan waitTime)
            => _totalWriteWaitTime.Inc(waitTime.TotalSeconds);

        /// <inheritdoc/>
        public void DecrementPendingWrites()
            => _pendingWrites.Inc();

        /// <inheritdoc/>
        public void IncrementPendingWrites()
            => _pendingWrites.Dec();

        /// <inheritdoc/>
        public void IncrementTotalCancelledConnections()
            => _totalCancelledConnections.Inc();

        /// <inheritdoc/>
        public void IncrementTotalEmptyMessagesReceived()
            => _totalEmptyMessagesReceived.Inc();

        /// <inheritdoc/>
        public void IncrementTotalFailedRequestCallbacks()
            => _totalFailedRequestCallbacks.Inc();

        /// <inheritdoc/>
        public void IncrementTotalFailedRequestHandlers()
            => _totalFailedRequestHandlers.Inc();

        /// <inheritdoc/>
        public void IncrementTotalFailedResponseWrites()
            => _totalFailedResponseWrites.Inc();

        /// <inheritdoc/>
        public void IncrementTotalPingsReceived()
            => _totalPingsReceived.Inc();

        /// <inheritdoc/>
        public void IncrementTotalPingTimeouts()
            => _totalPingTimeouts.Inc();

        /// <inheritdoc/>
        public void IncrementTotalPongsSent()
            => _totalPongsSent.Inc();

        /// <inheritdoc/>
        public void IncrementTotalReceivedMessages()
            => _totalReceivedMessages.Inc();

        /// <inheritdoc/>
        public void IncrementTotalRequestsReceived()
            => _totalReceivedRequests.Inc();

        /// <inheritdoc/>
        public void IncrementTotalStartedConnections()
            => _totalStartedConnections.Inc();

        /// <inheritdoc/>
        public void IncrementTotalWrites()
            => _totalWrites.Inc();
    }
}