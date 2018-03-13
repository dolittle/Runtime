/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
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
