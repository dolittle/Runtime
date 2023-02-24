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

    /// <summary>
    ///  The current event was included in the stream, increment the stream position as well as the event log position
    /// </summary>
    /// <returns></returns>
    public ProcessingPosition IncrementWithStream()
    {
        return new(StreamPosition.Increment(), EventLogPosition.Increment());
    }

    /// <summary>
    /// The current event was not included in the stream, only increment the event log position
    /// </summary>
    /// <returns></returns>
    public ProcessingPosition IncrementEventLogOnly()
    {
        return this with { EventLogPosition = EventLogPosition.Increment() };
    }

    /// <summary>
    /// If the event log position has not been initialized,
    /// the processing position is not valid before the event log position has been retrieved based on the stream position
    /// </summary>
    /// <returns></returns>
    public bool HasStreamPositionOnly()
    {
        return StreamPosition.Value > 0 && EventLogPosition.Value == 0;
    }
}
