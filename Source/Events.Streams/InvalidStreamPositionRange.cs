// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Exception that gets thrown when an invalid <see cref="StreamPosition" /> range is given.
    /// </summary>
    public class InvalidStreamPositionRange : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStreamPositionRange"/> class.
        /// </summary>
        /// <param name="from">The from <see cref="StreamPosition" />.</param>
        /// <param name="to">The to <see cref="StreamPosition" />.</param>
        public InvalidStreamPositionRange(StreamPosition from, StreamPosition to)
            : base($"From position '{from} is greater than '{to}'")
        {
        }
    }
}