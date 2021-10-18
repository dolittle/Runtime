// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Protobuf;
namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IExceptionToFailureConverter"/>
    /// </summary>
    public class ExceptionToFailureConverter : IExceptionToFailureConverter
    {
        /// <inheritdoc />
        public Failure ToFailure(Exception exception)
            => new(GetFailureId(exception), exception.Message);

        static FailureId GetFailureId(Exception exception)
            => exception switch
            {
                EventHandlerNotRegistered => EventHandlersFailures.EventHandlerNotRegistered,
                StreamProcessorNotRegisteredForTenant => EventHandlersFailures.EventHandlerNotRegisteredForTenant,
                AlreadySettingNewStreamProcessorPosition => EventHandlersFailures.AlreadySettingEventHandlerPosition,
                CannotSetStreamProcessorPositionHigherThanCurrentPosition => EventHandlersFailures.CannotReprocessEventsFromPositionHigherThanCurrentPosition,
                _ => FailureId.Other
            };
    }
}