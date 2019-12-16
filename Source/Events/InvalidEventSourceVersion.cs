// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Indicates that an <see cref="EventSourceVersion" /> was invalid (e.g. trying to increment the Sequence Number of the NoVersion or get the Previous Version of NoVersion).
    /// </summary>
    public class InvalidEventSourceVersion : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEventSourceVersion"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public InvalidEventSourceVersion(string message)
            : base(message)
        {
        }
    }
}