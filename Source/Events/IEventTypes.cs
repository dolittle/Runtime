// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events;

/// <summary>
/// Defines a system that knows about Event Types.
/// </summary>
public interface IEventTypes
{
    /// <summary>
    /// Gets all registered Event Types.
    /// </summary>
    IEnumerable<EventType> All { get; }

    /// <summary>
    /// Attempts to get the Event Type registered for an event type id.
    /// </summary>
    /// <param name="id">The <see cref="ArtifactId">event type id</see> to get the event type for.</param>
    /// <param name="eventType">When the method returns, contains the <see cref="EventType"/> associated with the id if found, or the default value if not.</param>
    /// <returns>True if an Event Type is registered for the id, false if not.</returns>
    bool TryGetFor(ArtifactId id, out EventType? eventType);
        
    /// <summary>
    /// Registers an Event Type.
    /// </summary>
    /// <param name="eventType">The Event Type to register.</param>
    void Register(EventType eventType);
}
