// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Represents an implementatino of <see cref="IMetricsCollector"/>.
    /// </summary>
    public class MetricsCollector : IMetricsCollector
    {
        /// <inheritdoc/>
        public void AddToTotalStreamWriteTime(TimeSpan writeTime)
        {
        }

        /// <inheritdoc/>
        public void AddToTotalStreamWriteWaitTime(TimeSpan waitTime)
        {
        }

        /// <inheritdoc/>
        public void AddToTotalWaitForFirstMessageTime(TimeSpan waitTime)
        {
        }

        /// <inheritdoc/>
        public void AddToTotalWaitForPingStarterToCompleteTime(TimeSpan waitTime)
        {
        }

        /// <inheritdoc/>
        public void DecrementPendingStreamWrites()
        {
        }

        /// <inheritdoc/>
        public void IncrementPendingStreamWrites()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalKeepaliveTimeouts()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalKeepaliveTokenResets()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalPingsSent()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalPongsReceived()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalStreamReadBytes(int writtenBytes)
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalStreamReads()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalStreamWriteBytes(int writtenBytes)
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalStreamWrites()
        {
        }
    }
}
