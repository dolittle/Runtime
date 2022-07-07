// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Defines an event that is stored in a collection.
/// </summary>
public interface IStoredEvent
{
    /// <summary>
    /// Gets the <see cref="EventLogSequenceNumber"/>.
    /// </summary>
    /// <returns></returns>
    EventLogSequenceNumber GetEventLogSequenceNumber();

    /// <summary>
    /// Whether two <see cref="IStoredEvent"/> events represents the same event.
    /// </summary>
    /// <param name="otherEvent">The other <see cref="IStoredEvent"/>.</param>
    /// <returns>True if same, false if not.</returns>
    bool IsTheSameAs(IStoredEvent otherEvent);
}
