// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Exception that gets thrown when there was no event in <see cref="StreamId" /> at <see cref="StreamPosition" />.
    /// </summary>
    public class NoEventInStreamAtPosition : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventInStreamAtPosition"/> class.
        /// </summary>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition" />.</param>
        public NoEventInStreamAtPosition(StreamId streamId, StreamPosition streamPosition)
            : base($"No event in stream '{streamId.Value} at position '{streamPosition.Value}'")
        {
        }
    }
}