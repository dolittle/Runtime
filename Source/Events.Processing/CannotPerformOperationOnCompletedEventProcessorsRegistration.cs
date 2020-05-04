// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when trying to perform operation on <see cref="IEventProcessorsRegistration" /> after it has been completed.
    /// </summary>
    public class CannotPerformOperationOnCompletedEventProcessorsRegistration : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotPerformOperationOnCompletedEventProcessorsRegistration"/> class.
        /// </summary>
        public CannotPerformOperationOnCompletedEventProcessorsRegistration()
            : base("Cannot perform operation on completed Event Processors Registration")
        {
        }
    }
}