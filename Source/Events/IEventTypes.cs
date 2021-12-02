// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

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
    /// Registers an Event Type.
    /// </summary>
    /// <param name="eventType">The Event Type to register.</param>
    void Register(EventType eventType);
}