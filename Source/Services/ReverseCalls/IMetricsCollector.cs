// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Defines a system for collecting metrics about reverse calls.
    /// </summary>
    public interface IMetricsCollector
    {
        void IncrementPendingStreamWrites();
        void DecrementPendingStreamWrites();
        void IncrementTotalStreamWrites();
        void IncrementTotalStreamWriteBytes(int writtenBytes);
        void AddToTotalStreamWriteWaitTime(TimeSpan waitTime);
        void AddToTotalStreamWriteTime(TimeSpan writeTime);
        void IncrementTotalStreamReads();
        void IncrementTotalStreamReadBytes(int writtenBytes);
        void IncrementTotalPingsSent();
        void IncrementTotalPongsReceived();
        void IncrementTotalKeepaliveTokenResets();
        void IncrementTotalKeepaliveTimeouts();
        void AddToTotalWaitForFirstMessageTime(TimeSpan waitTime);
    }
}
