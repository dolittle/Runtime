// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.ReverseCalls
{
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
        void AddToTotalWaitForPingStarterToCompleteTime(TimeSpan waitTime);
    }

    public class MetricsCollector : IMetricsCollector
    {
        public void AddToTotalStreamWriteTime(TimeSpan writeTime) { }
        public void AddToTotalStreamWriteWaitTime(TimeSpan waitTime) { }
        public void DecrementPendingStreamWrites() { }
        public void IncrementPendingStreamWrites() { }
        public void IncrementTotalPingsSent() { }
        public void IncrementTotalPongsReceived() { }
        public void IncrementTotalStreamReadBytes(int writtenBytes) { }
        public void IncrementTotalStreamReads() { }
        public void IncrementTotalStreamWriteBytes(int writtenBytes) { }
        public void IncrementTotalStreamWrites() { }
    }
}