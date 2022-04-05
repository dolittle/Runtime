// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents an uncommitted aggregate event.
/// </summary>
/// <param name="Type">The event type.</param>
/// <param name="Public">Whether the event is public.</param>
/// <param name="Content">The content of the Event as a JSON-encoded string.</param>
public record UncommittedAggregateEvent(
    Artifact Type,
    bool Public,
    string Content)
{
    /// <summary>
    /// Converts an <see cref="UncommittedAggregateEvent"/> to an <see cref="Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent"/>.
    /// </summary>
    /// <param name="event">The event to convert.</param>
    /// <returns>The converted event.</returns>
    public static implicit operator Contracts.UncommittedAggregateEvents.Types.UncommittedAggregateEvent(UncommittedAggregateEvent @event)
        => new()
        {
            EventType = @event.Type,
            Public = @event.Public,
            Content = @event.Content,
        };
}
