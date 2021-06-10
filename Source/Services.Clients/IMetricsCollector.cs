// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Defines a system for collecting metrics about reverse call clients.
    /// </summary>
    public interface IMetricsCollector
    {
        /// <summary>
        /// Increments the number of started connections.
        /// </summary>
        void IncrementTotalStartedConnections();

        /// <summary>
        /// Increments the number of current pending stream writes.
        /// </summary>
        void IncrementPendingWrites();

        /// <summary>
        /// Decrements the number of current pending stream writes.
        /// </summary>
        void DecrementPendingWrites();

        /// <summary>
        /// Adds to the total time waiting for a stream to be available for writing.
        /// </summary>
        /// <param name="waitTime">The time spent waiting.</param>
        void AddToTotalWriteWaitTime(TimeSpan waitTime);

        /// <summary>
        /// Adds to the total time spent writing to a stream.
        /// </summary>
        /// <param name="waitTime">The time spent writing.</param>
        void AddToTotalWriteTime(TimeSpan writeTime);

        /// <summary>
        /// Increments the number of total writes to a stream.
        /// </summary>
        void IncrementTotalWrites();

        /// <summary>
        /// Adds to the total number of bytes written to a stream.
        /// </summary>
        /// <param name="messageSize">The size of the message in bytes.</param>
        void AddToTotalWriteBytes(int messageSize);

        /// <summary>
        /// Adds to the time spent waiting for a connect response message.
        /// </summary>
        /// <param name="waitTime">The time spent waiting.</param>
        void AddToTotalWaitForConnectResponseTime(TimeSpan waitTime);

        /// <summary>
        /// Increments the total number of cancelled connections.
        /// </summary>
        void IncrementTotalCancelledConnections();

        /// <summary>
        /// Increments the total number of received messages
        /// </summary>
        void IncrementTotalReceivedMessages();

        /// <summary>
        /// Adds to the total number of bytes read from a stream.
        /// </summary>
        /// <param name="messageSize">The size of the message in bytes.</param>
        void AddToTotalReceivedBytes(int messageSize);

        /// <summary>
        /// Increments the total number of pings received.
        /// </summary>
        void IncrementTotalPingsReceived();

        /// <summary>
        /// Increments the total number of pongs sent.
        /// </summary>
        void IncrementTotalPongsSent();

        /// <summary>
        /// Increments the total number of requests received.
        /// </summary>
        void IncrementTotalRequestsReceived();

        /// <summary>
        /// Increments the total number of empty messages received.
        /// </summary>
        void IncrementTotalEmptyMessagesReceived();

        /// <summary>
        /// Increments the total number of connections that timed out because of lost pings.
        /// </summary>
        void IncrementTotalPingTimeouts();

        /// <summary>
        /// Increments the total number of failed request handlers.
        /// </summary>
        void IncrementTotalFailedRequestHandlers();

        /// <summary>
        /// Increments the total number of failed request callbacks.
        /// </summary>
        void IncrementTotalFailedRequestCallbacks();

        /// <summary>
        /// Increments the total number of failed response writes.
        /// </summary>
        void IncrementTotalFailedResponseWrites();

        /// <summary>
        /// Adds to the total times spent handling requests.
        /// </summary>
        /// <param name="handleTime">The time spent handling a request.</param>
        void AddToTotalRequestHandlingTime(TimeSpan handleTime);
    }
}
