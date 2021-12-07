// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Exception that gets thrown when an <see cref="EventHandler{TEventArgs}"/> has not been registered.
/// </summary>
public class EventHandlerNotRegistered : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="EventHandlerNotRegistered"/> class.
    /// </summary>
    /// <param name="eventHandler">The <see cref="EventHandlerId"/>.</param>
    public EventHandlerNotRegistered(EventHandlerId eventHandler)
        : base($"Event handler {eventHandler} is not registered")
    {
    }
}