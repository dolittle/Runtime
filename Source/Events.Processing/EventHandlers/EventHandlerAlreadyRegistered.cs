// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="EventHandler{TEventArgs}"/> has already been registered.
    /// </summary>
    public class EventHandlerAlreadyRegistered : Exception
    {
        /// <summary>
        /// Initializes an instance of the <see cref="EventHandlerAlreadyRegistered"/> class.
        /// </summary>
        /// <param name="eventHandler">The <see cref="EventHandlerId"/>.</param>
        public EventHandlerAlreadyRegistered(EventHandlerId eventHandler)
            : base($"Event handler {eventHandler} is already registered")
        {
        }
    }
}