// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="EventStream" /> is created with no events.
    /// </summary>
    public class InvalidEmptyEventStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEmptyEventStream"/> class.
        /// </summary>
        public InvalidEmptyEventStream()
            : base("An event stream should not be empty")
        {
        }
    }
}