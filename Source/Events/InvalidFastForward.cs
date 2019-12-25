// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents an exceptional situation where an event source is stateful
    /// but there has been an attempt to retrieve it without restoring state by replaying events (fast-forwarding).
    /// </summary>
    public class InvalidFastForward : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFastForward"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public InvalidFastForward(string message)
            : base(message)
        {
        }
    }
}