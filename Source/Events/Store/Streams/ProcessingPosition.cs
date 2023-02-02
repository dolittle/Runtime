// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// 
/// </summary>
/// <param name="StreamPosition">Number of processed events in the stream</param>
/// <param name="EventLogPosition">The current event log position the stream has reached</param>
public record ProcessingPosition(StreamPosition StreamPosition, EventLogSequenceNumber EventLogPosition) : IComparable<ProcessingPosition>
{
    public static readonly ProcessingPosition Initial = new(StreamPosition.Start, EventLogSequenceNumber.Initial);

    public int CompareTo(ProcessingPosition? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var eventLogPositionComparison = EventLogPosition.CompareTo(other.EventLogPosition);
        return eventLogPositionComparison != 0 ? eventLogPositionComparison : EventLogPosition.CompareTo(other.EventLogPosition);
    }

    
    public ProcessingPosition IncrementProcessed()
    {
        return new(StreamPosition.Increment(), EventLogPosition.Increment());
    }

    public ProcessingPosition IncrementNonProcessed()
    {
        return this with { EventLogPosition = EventLogPosition.Increment() };
    }
}
