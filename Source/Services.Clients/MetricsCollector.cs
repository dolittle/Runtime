// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Represents an implementation of <see cref="IMetricsCollector"/>.
    /// </summary>
    public class MetricsCollector : IMetricsCollector
    {
        public void AddToTotalReceivedBytes(int messageSize)
        {
        }

        public void AddToTotalRequestHandlingTime(TimeSpan handleTime)
        {
        }

        public void AddToTotalWaitForConnectResponseTime(TimeSpan waitTime)
        {
        }

        public void AddToTotalWriteBytes(int messageSize)
        {
        }

        public void AddToTotalWriteTime(TimeSpan writeTime)
        {
        }

        public void AddToTotalWriteWaitTime(TimeSpan waitTime)
        {
        }

        public void DecrementPendingWrites()
        {
        }

        public void IncrementPendingWrites()
        {
        }

        public void IncrementTotalCancelledConnections()
        {
        }

        public void IncrementTotalEmptyMessagesReceived()
        {
        }

        public void IncrementTotalFailedRequestCallbacks()
        {
        }

        public void IncrementTotalFailedRequestHandlers()
        {
        }

        public void IncrementTotalFailedResponseWrites()
        {
        }

        public void IncrementTotalPingsReceived()
        {
        }

        public void IncrementTotalPingTimeouts()
        {
        }

        public void IncrementTotalPongsSent()
        {
        }

        public void IncrementTotalReceivedMessages()
        {
        }

        public void IncrementTotalRequestsReceived()
        {
        }

        public void IncrementTotalStartedConnections()
        {
        }

        public void IncrementTotalWrites()
        {
        }

    }
}