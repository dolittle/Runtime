// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Defines the protocol for event handlers.
/// </summary>
public interface IEventHandlersProtocol : IReverseCallServiceProtocol<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse, EventHandlerRegistrationArguments>
{
}