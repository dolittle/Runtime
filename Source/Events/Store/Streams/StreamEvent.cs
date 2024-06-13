// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents a <see cref="CommittedEvent" /> that is a part of a stream.
/// </summary>
public record StreamEvent(CommittedEvent Event, StreamPosition Position, StreamId Stream, PartitionId Partition, bool Partitioned, EventLogSequenceNumber NextSequenceInStream)
{
    public StreamEvent(CommittedEvent @event, StreamPosition position, StreamId stream, PartitionId partition, bool partitioned)
        : this(@event, position, stream, partition, partitioned, @event.EventLogSequenceNumber.Increment())
    {
    }
    
    public ProcessingPosition NextProcessingPosition => new(Position.Increment(), NextSequenceInStream);
    public ProcessingPosition CurrentProcessingPosition { get; } = new(Position, Event.EventLogSequenceNumber);
}
