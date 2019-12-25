// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Events
{
    /// <summary>
    /// Defines the basics of an event.
    /// </summary>
    /// <remarks>
    /// Types inheriting from this interface can be used in event sourcing and will be picked up by the event migration system.
    /// </remarks>
    public interface IEvent
    {
    }
}
