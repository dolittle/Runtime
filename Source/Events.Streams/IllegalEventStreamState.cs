// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="EventStreamState" /> is in an illegal <see cref="StreamState" />.
    /// </summary>
    public class IllegalEventStreamState : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IllegalEventStreamState"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public IllegalEventStreamState(StreamState state)
        : base($"{state} is an illegal {typeof(StreamState).FullName}")
        {
        }
    }
}