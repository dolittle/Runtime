// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when there are no filters registered for stream <see cref="StreamId" />.
    /// </summary>
    public class NoFilterRegisteredForStream : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoFilterRegisteredForStream"/> class.
        /// </summary>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        public NoFilterRegisteredForStream(StreamId streamId)
            : base($"There are no filter registered for stream '{streamId}'")
        {
        }
    }
}