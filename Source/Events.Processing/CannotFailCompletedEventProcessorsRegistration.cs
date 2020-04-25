// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when <see cref="IEventProcessorsRegistration.Fail()" /> is called on a completed <see cref="IEventProcessorsRegistration" />.
    /// </summary>
    public class CannotFailCompletedEventProcessorsRegistration : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotFailCompletedEventProcessorsRegistration"/> class.
        /// </summary>
        public CannotFailCompletedEventProcessorsRegistration()
            : base($"Cannot fail an already completed Event Processor Registration")
        {
        }
    }
}