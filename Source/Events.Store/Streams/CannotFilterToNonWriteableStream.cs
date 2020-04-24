// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Exception that gets thrown when a filter is filtering to a non-writeable <see cref="StreamId">stream</see>.
    /// </summary>
    public class CannotFilterToNonWriteableStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotFilterToNonWriteableStream"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public CannotFilterToNonWriteableStream(StreamId stream)
            : base($"Cannot create a filter that writes to the non-writeable stream '{stream}'")
        {
        }
    }
}