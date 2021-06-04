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
        void IncrementTotalStartedConnections();
        void IncrementPendingWrites();
        void DecrementPendingWrites();
        void AddToTotalWriteWaitTime(TimeSpan waitTime);
        void AddToTotalWriteTime(TimeSpan writeTime);
        void IncrementTotalWrites();
        void AddToTotalWriteBytes(int messageSize);
        void AddToTotalWaitForConnectResponseTime(TimeSpan waitTime);
        void IncrementTotalCancelledConnections();
        void IncrementTotalReceivedMessages();
        void AddToTotalReceivedBytes(int messageSize);
        void IncrementTotalPingsReceived();
        void IncrementTotalPongsSent();
        void IncrementTotalRequestsReceived();
        void IncrementTotalEmptyMessagesReceived();
        void IncrementTotalPingTimeouts();
        void IncrementTotalFailedRequestHandlers();
        void IncrementTotalFailedRequestCallbacks();
        void IncrementTotalFailedResponseWrites();
        void AddToTotalRequestHandlingTime(TimeSpan handleTime);
    }
}
