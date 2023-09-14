// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned;

/// <summary>
/// Represents the state of a failing partition.
/// </summary>
/// <param name="Position">The <see cref="ProcessingPosition" />.</param>
/// <param name="RetryTime">The <see cref="DateTimeOffset" /> to retry processing.</param>
/// <param name="Reason">The reason for failing.</param>
/// <param name="ProcessingAttempts">The number of times the Event has been processed.</param>
/// <param name="LastFailed">The <see cref="DateTimeOffset" /> for when this partition last failed.</param>
public record FailingPartitionState(ProcessingPosition Position, DateTimeOffset RetryTime, string Reason,
    uint ProcessingAttempts, DateTimeOffset LastFailed)
{
    public FailingPartitionState(StreamPosition position, EventLogSequenceNumber retryTime, DateTimeOffset toDateTimeOffset, string failureReason,
        uint processingAttempts, DateTimeOffset dateTimeOffset)
        : this(new ProcessingPosition(position, retryTime), toDateTimeOffset, failureReason, processingAttempts, dateTimeOffset)
    {
    }

    [Obsolete("legacy stream processor state, without event log position")]
    public FailingPartitionState(StreamPosition streamPosition, DateTimeOffset retryTime, string reason,
        uint processingAttempts, DateTimeOffset lastFailed) : this(new ProcessingPosition(streamPosition, EventLogSequenceNumber.Initial), retryTime, reason,
        processingAttempts, lastFailed)
    {
    }

    public bool CanBeRetried => RetryTime < DateTimeOffset.MaxValue;

    public bool TryGetTimespanToRetry(out TimeSpan timeToRetry)
    {
        if (!CanBeRetried)
        {
            timeToRetry = default;
            return false;
        }

        var now = DateTimeOffset.UtcNow;

        if (RetryTime > now)
        {
            timeToRetry = RetryTime.Subtract(now);
            return true;
        }

        timeToRetry = TimeSpan.Zero;
        return true;
    }

    public FailingPartitionState SkipEventsBefore(EventLogSequenceNumber eventLogSequence)
    {
        if (Position.EventLogPosition >= eventLogSequence)
        {
            // Already at or after the given position
            return this;
        }

        // Skip events before the given position
        return this with
        {
            Position = Position with { EventLogPosition = eventLogSequence },
            ProcessingAttempts = 0,
            Reason = "Skipped older events in the partition",
        };
    }
}
