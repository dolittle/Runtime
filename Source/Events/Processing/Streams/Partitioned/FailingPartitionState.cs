// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned;

/// <summary>
/// Represents the state of a failing partition.
/// </summary>
/// <param name="Position">The <see cref="StreamPosition" />.</param>
/// <param name="EventLogPosition">The <see cref="EventLogSequenceNumber" />.</param>
/// <param name="RetryTime">The <see cref="DateTimeOffset" /> to retry processing.</param>
/// <param name="Reason">The reason for failing.</param>
/// <param name="ProcessingAttempt">The number of times the Event has been processed.</param>
/// <param name="LastFailed">The <see cref="DateTimeOffset" /> for when this partition last failed.</param>
public record FailingPartitionState(StreamPosition Position, EventLogSequenceNumber EventLogPosition, DateTimeOffset RetryTime, string Reason,
    uint ProcessingAttempts, DateTimeOffset LastFailed)
{
    public FailingPartitionState(ProcessingPosition position, DateTimeOffset retryTime, string reason, uint processingAttempts, DateTimeOffset lastFailed) : this(
        position.StreamPosition, position.EventLogPosition, retryTime, reason, processingAttempts, lastFailed)
    {
    }

    public ProcessingPosition ProcessingPosition => new(Position, EventLogPosition);
}
