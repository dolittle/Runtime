// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when <see cref="IEventProcessorsRegistration.Register()" /> is called after it has been completed.
    /// </summary>
    public class CannotRegisterOnCompletedEventProcessorsRegistration : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotRegisterOnCompletedEventProcessorsRegistration"/> class.
        /// </summary>
        public CannotRegisterOnCompletedEventProcessorsRegistration()
            : base("Cannot register compelted Event Processors Registration")
        {
        }
    }
}