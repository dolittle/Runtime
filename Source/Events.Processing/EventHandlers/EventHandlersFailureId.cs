// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents the unique identifier of an event handlers failure type.
    /// </summary>
    public class EventHandlersFailureId : FailureId
    {
        /// <summary>
        /// Gets the <see cref="EventHandlersFailureId" /> that represents the 'NoEventHandlerRegistration' failure type.
        /// </summary>
        public static EventHandlersFailureId NoEventHandlerRegistration => new EventHandlersFailureId { Value = Guid.Parse("209a79c7-824c-4988-928b-0dd517746ca0") };

        /// <summary>
        /// Gets the <see cref="EventHandlersFailureId" /> that represents the 'CannotRegisterEventHandlerOnNonWriteableStream' failure type.
        /// </summary>
        public static EventHandlersFailureId CannotRegisterEventHandlerOnNonWriteableStream => new EventHandlersFailureId { Value = Guid.Parse("45b4c918-37a5-405c-9865-d032869b1d24") };

        /// <summary>
        /// Gets the <see cref="EventHandlersFailureId" /> that represents the 'FailedToRegisterEventHandler' failure type.
        /// </summary>
        public static EventHandlersFailureId FailedToRegisterEventHandler => new EventHandlersFailureId { Value = Guid.Parse("dbfdfa15-e727-49f6-bed8-7a787954a4c6") };
    }
}
