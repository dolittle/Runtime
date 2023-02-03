// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents a <see cref="CommittedEvent" /> that is a part of a stream.
/// </summary>
public record StreamEvent(CommittedEvent Event, StreamPosition Position, StreamId Stream, PartitionId Partition, bool Partitioned)
{
    public ProcessingPosition NextProcessingPosition => new(Position.Increment(), Event.EventLogSequenceNumber.Increment());
    public ProcessingPosition CurrentProcessingPosition => new(Position, Event.EventLogSequenceNumber);
}
