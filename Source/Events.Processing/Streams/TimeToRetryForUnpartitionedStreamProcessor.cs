// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanGetTimeToRetryFor{T}" /> <see cref="StreamProcessorState" />.
    /// </summary>
    public class TimeToRetryForUnpartitionedStreamProcessor : ICanGetTimeToRetryFor<StreamProcessorState>
    {
        /// <inheritdoc/>
        public bool TryGetTimespanToRetry(StreamProcessorState streamProcessorState, out TimeSpan timeToRetry)
        {
            timeToRetry = TimeSpan.MaxValue;
            if (streamProcessorState.IsFailing)
            {
                var retryTime = streamProcessorState.RetryTime;
                timeToRetry = RetryTimeIsInThePast(retryTime) ? TimeSpan.Zero : retryTime.Subtract(DateTimeOffset.UtcNow);
                return true;
            }

            return false;
        }

        bool RetryTimeIsInThePast(DateTimeOffset retryTime)
            => DateTimeOffset.UtcNow.CompareTo(retryTime) >= 0;
    }
}
