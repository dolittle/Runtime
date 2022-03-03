// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion.Lifecycle;


namespace Dolittle.Runtime.Events;

/// <summary>
/// Represents an implementation of <see cref="IEventTypes"/>.
/// </summary>
[Singleton]
public class EventTypes : IEventTypes
{
    readonly ConcurrentDictionary<ArtifactId, EventType> _eventTypes = new();

    /// <inheritdoc />
    public IEnumerable<EventType> All => _eventTypes.Values;

    /// <inheritdoc />
    public void Register(EventType eventType)
        => _eventTypes.AddOrUpdate(eventType.Identifier.Id, eventType, (_, _) => eventType);
}
