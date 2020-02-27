// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Exception that gets thrown when there are multiple well-known stream writers that can write to the <see cref="StreamId" />.
    /// </summary>
    public class MultipleWellKnownStreamWriters : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleWellKnownStreamWriters"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public MultipleWellKnownStreamWriters(StreamId stream)
            : base($"There are multiple instance of {typeof(ICanWriteEventsToWellKnownStreams).FullName} that can write to stream '{stream}'")
        {
        }
    }
}