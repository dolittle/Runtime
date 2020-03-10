// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Exception that gets thrown when the <see cref="StreamPositionRange" />.
    /// </summary>
    public class StreamPositionRangeIsOutOfBounds : InvalidStreamPositionRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamPositionRangeIsOutOfBounds"/> class.
        /// </summary>
        /// <param name="range">The out of bounds <see cref="StreamPositionRange" />.</param>
        /// <param name="validRange">The valid range <see cref="StreamPositionRange" />.</param>
        public StreamPositionRangeIsOutOfBounds(StreamPositionRange range, StreamPositionRange validRange)
            : base(range, $"Range is out of bounds. Expected range to be in range {validRange}")
        {
        }
    }
}