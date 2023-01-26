// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned;

/// <summary>
/// Represents an implementation of <see cref="ICanGetTimeToRetryFor{T}" /> <see cref="StreamProcessorState" />.
/// </summary>
public class TimeToRetryForPartitionedStreamProcessor : ICanGetTimeToRetryFor<StreamProcessorState>
{
    /// <inheritdoc/>
    public bool TryGetTimespanToRetry(StreamProcessorState streamProcessorState, out TimeSpan timeToRetry)
    {
        timeToRetry = TimeSpan.MaxValue;
        var failingStates = streamProcessorState.FailingPartitions;
        if (failingStates.Count > 0)
        {
            var earliestRetryTime = failingStates.Min(_ => _.Value.RetryTime);
            timeToRetry = RetryTimeIsInThePast(earliestRetryTime) ? TimeSpan.Zero : earliestRetryTime.Subtract(DateTimeOffset.UtcNow);
            return true;
        }

        return false;
    }

    bool RetryTimeIsInThePast(DateTimeOffset retryTime)
        => DateTimeOffset.UtcNow.CompareTo(retryTime) >= 0;
}