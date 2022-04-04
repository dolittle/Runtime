// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

/// <summary>
/// Represents an uncommitted event.
/// </summary>
/// <param name="EventSource">The Event Source of the Event.</param>
/// <param name="Type">The Event Type artifact.</param>
/// <param name="Public">Whether the Event is public.</param>
/// <param name="Content">The content of the Event as a JSON-encoded string.</param>
public record UncommittedEvent(
    string EventSource, 
    Artifact Type, 
    bool Public, 
    string Content)
{
    
    /// <summary>
    /// Converts an <see cref="UncommittedEvent"/> to an <see cref="Store.UncommittedEvent"/>.
    /// </summary>
    /// <param name="event">The uncommitted event to convert.</param>
    /// <returns>The converted uncommitted event.</returns>
    public static implicit operator Store.UncommittedEvent(UncommittedEvent @event)
        => new(@event.EventSource, @event.Type, @event.Public, @event.Content);
}
