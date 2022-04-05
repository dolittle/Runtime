// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents a committed event.
/// </summary>
/// <param name="EventLogSequenceNumber">The Event Log sequence number of the committed event.</param>
/// <param name="Occurred">When the event was committed.</param>
/// <param name="EventSource">The event source that committed the event.</param>
/// <param name="ExecutionContext">The execution context the event was committed in.</param>
/// <param name="Type">The artifact type of the event.</param>
/// <param name="Public">Whether the event is public.</param>
/// <param name="Content">The content of the Event as a JSON-encoded string.</param>
public record CommittedEvent(
    ulong EventLogSequenceNumber,
    DateTimeOffset Occurred,
    string EventSource,
    ExecutionContext ExecutionContext,
    Artifact Type,
    bool Public,
    string Content
)
{
    /// <summary>
    /// Converts a <see cref="Contracts.CommittedEvent"/> to a <see cref="CommittedEvent"/>.
    /// </summary>
    /// <param name="event">The event to convert.</param>
    /// <returns>The converted event.</returns>
    public static implicit operator CommittedEvent(Contracts.CommittedEvent @event)
        => new(
            @event.EventLogSequenceNumber,
            @event.Occurred.ToDateTimeOffset(),
            @event.EventSourceId,
            @event.ExecutionContext,
            @event.EventType,
            @event.Public,
            @event.Content);
}
