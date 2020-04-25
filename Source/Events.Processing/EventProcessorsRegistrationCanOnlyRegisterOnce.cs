// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when <see cref="IEventProcessorsRegistration.Register()" /> is called multiple times.
    /// </summary>
    public class EventProcessorsRegistrationCanOnlyRegisterOnce : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorsRegistrationCanOnlyRegisterOnce"/> class.
        /// </summary>
        public EventProcessorsRegistrationCanOnlyRegisterOnce()
            : base("Event Processors Registration cannot register more than once")
        {
        }
    }
}