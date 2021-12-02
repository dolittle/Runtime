// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Holds the unique <see cref="FailureId"> failure ids </see> unique to the Event Handlers.
/// </summary>
public static class EventHandlersFailures
{
    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'NoEventHandlerRegistrationReceived' failure type.
    /// </summary>
    public static FailureId NoEventHandlerRegistrationReceived => FailureId.Create("209a79c7-824c-4988-928b-0dd517746ca0");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'CannotRegisterEventHandlerOnNonWriteableStream' failure type.
    /// </summary>
    public static FailureId CannotRegisterEventHandlerOnNonWriteableStream => FailureId.Create("45b4c918-37a5-405c-9865-d032869b1d24");

    /// <summary>
    /// Gets the <see cref="FailureId" /> that represents the 'FailedToRegisterEventHandler' failure type.
    /// </summary>
    public static FailureId FailedToRegisterEventHandler => FailureId.Create("dbfdfa15-e727-49f6-bed8-7a787954a4c6");
}