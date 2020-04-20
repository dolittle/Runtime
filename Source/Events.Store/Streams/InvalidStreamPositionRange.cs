// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Exception that gets thrown when there is an invalid <see cref="StreamPositionRange" />.
    /// </summary>
    public class InvalidStreamPositionRange : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStreamPositionRange"/> class.
        /// </summary>
        /// <param name="range">The <see cref="StreamPositionRange" />.</param>
        /// <param name="reason"> The reason why this is an illegal range.</param>
        public InvalidStreamPositionRange(StreamPositionRange range, InvalidStreamPositionRangeReason reason)
            : base($"The range {range} is an invalid stream position range. {reason}")
        {
        }
    }
}