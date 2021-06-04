// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.Callbacks
{
    /// <summary>
    /// Represents an implementatino of <see cref="IMetricsCollector"/>.
    /// </summary>
    public class MetricsCollector : IMetricsCollector
    {
        /// <inheritdoc/>
        public void AddToTotalCallbackTime(TimeSpan elapsed)
        {
        }

        /// <inheritdoc/>
        public void AddToTotalSchedulesMissedTime(TimeSpan elapsed)
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalCallbackLoopsFailed()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalCallbacksCalled()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalCallbacksFailed()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalCallbacksRegistered()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalCallbacksUnregistered()
        {
        }

        /// <inheritdoc/>
        public void IncrementTotalSchedulesMissed()
        {
        }
    }
}
