// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Extension methods for <see cref="IEventTypes"/>.
/// </summary>
public static class EventTypesExtensions
{
    /// <summary>
    /// Gets the registered Alias for an EventType as a string, or the empty string if it is not set.
    /// </summary>
    /// <param name="eventTypes">The <see cref="IEventTypes"/> to use to resolve the registration.</param>
    /// <param name="eventType">The <see cref="Artifact">event type</see> to find alias for.</param>
    /// <returns>The Alias for the EventType if found, or empty string.</returns>
    public static string GetEventTypeAliasOrEmptyString(this IEventTypes eventTypes, Artifact eventType)
    {
        if (!eventTypes.TryGetFor(eventType.Id, out var eventTypeInfo))
        {
            return string.Empty;
        }

        var alias = eventTypeInfo!.Alias;
        return alias == EventTypeAlias.NotSet
            ? string.Empty
            : alias.Value;
    }
}
