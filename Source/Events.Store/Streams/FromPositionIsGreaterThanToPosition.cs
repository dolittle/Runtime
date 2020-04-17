// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Exception that gets thrown when the from <see cref="StreamPosition" /> is greater than the to <see cref="StreamPosition" /> in a <see cref="StreamPositionRange" />.
    /// </summary>
    public class FromPositionIsGreaterThanToPosition : InvalidStreamPositionRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FromPositionIsGreaterThanToPosition"/> class.
        /// </summary>
        /// <param name="range">The <see cref="StreamPositionRange" />.</param>
        public FromPositionIsGreaterThanToPosition(StreamPositionRange range)
            : base(range, $"'From' position '{range.From} is greater than 'to' position '{range.To}'.")
        {
        }
    }
}