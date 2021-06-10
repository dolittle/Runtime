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
        /// <summary>
        /// Increments the number of current pending stream writes.
        /// </summary>
        void IncrementPendingStreamWrites();

        /// <summary>
        /// Decrements the number of current pending stream writes.
        /// </summary>
        void DecrementPendingStreamWrites();

        /// <summary>
        /// Increments the number of total writes to a stream.
        /// </summary>
        void IncrementTotalStreamWrites();

        /// <summary>
        /// Adds to the total number of bytes written to a stream.
        /// </summary>
        /// <param name="writtenBytes">The number of bytes written.</param>
        void IncrementTotalStreamWriteBytes(int writtenBytes);

        /// <summary>
        /// Adds to the total time waiting for a stream to be available for writing.
        /// </summary>
        /// <param name="waitTime">The time spent waiting.</param>
        void AddToTotalStreamWriteWaitTime(TimeSpan waitTime);

        /// <summary>
        /// Adds to the total time spent writing to a stream.
        /// </summary>
        /// <param name="waitTime">The time spent writing.</param>
        void AddToTotalStreamWriteTime(TimeSpan writeTime);

        /// <summary>
        /// Increments the number of total reads from a stream.
        /// </summary>
        void IncrementTotalStreamReads();

        /// <summary>
        /// Adds to the total number of bytes read from a stream.
        /// </summary>
        /// <param name="writtenBytes">The number of bytes read.</param>
        void IncrementTotalStreamReadBytes(int writtenBytes);

        /// <summary>
        /// Increments the total number of pings sent.
        /// </summary>
        void IncrementTotalPingsSent();

        /// <summary>
        /// Increments the total number of pongs received.
        /// </summary>
        void IncrementTotalPongsReceived();

        /// <summary>
        /// Increments the total number of times the keepalive token was reset because a pong was received.
        /// </summary>
        void IncrementTotalKeepaliveTokenResets();

        /// <summary>
        /// Increments the total number of times that a keepalive timed out because pongs were not received.
        /// </summary>
        void IncrementTotalKeepaliveTimeouts();

        /// <summary>
        /// Adds to the total time waiting for the first message on a stream.
        /// </summary>
        /// <param name="waitTime">The time spent waiting.</param>
        void AddToTotalWaitForFirstMessageTime(TimeSpan waitTime);
    }
}
