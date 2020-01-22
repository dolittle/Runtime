// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when a sequence of events is created with no events.
    /// </summary>
    public class InvalidEmptyEventSequence : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEmptyEventSequence"/> class.
        /// </summary>
        public InvalidEmptyEventSequence()
            : base("An event sequence should not be empty")
        {
        }
    }
}