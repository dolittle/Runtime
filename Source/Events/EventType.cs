// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events;

/// <summary>
/// Represents the Event Type.
/// </summary>
/// <param name="Identifier">The Event Type <see cref="Artifact"/>.</param>
/// <param name="Alias">The Alias of the Event Type.</param>
public record EventType(Artifact Identifier, EventTypeAlias Alias)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventType"/> record.
    /// </summary>
    /// <param name="type"></param>
    public EventType(Artifact type)
        : this(type, EventTypeAlias.NotSet)
    {
    }

    /// <inheritdoc />
    public override string ToString()
        => Alias.Equals(EventTypeAlias.NotSet)
            ? Identifier.Id.Value.ToString()
            : Alias;
}