// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// The <see cref="Failure"/> that is returned when attempting to register an <see cref="EventHandler"/> that has already been registered.
/// </summary>
/// <param name="EventHandlerId">The event handler identifier.</param>
public record EventHandlerAlreadyRegistered(EventHandlerId EventHandlerId) : Failure(
    EventHandlersFailures.EventHandlerAlreadyRegistered,
    $"The event handler {EventHandlerId.EventHandler} in scope {EventHandlerId.Scope} is already registered"
);
