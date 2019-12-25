// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the error when an <see cref="EventStream" /> is created with no events.
    /// </summary>
    public class InvalidEmptyEventStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEmptyEventStream"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public InvalidEmptyEventStream(string message)
            : base(message)
        {
        }
    }
}