// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// An position used to identify the position, or offset, of a stream.
    /// </summary>
    public class StreamPosition : ConceptAs<ulong>
    {
        /// <summary>
        /// Represents the initial value of the <see cref="StreamPosition">position</see>.
        /// </summary>
        public static StreamPosition Start = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamPosition"/> class.
        /// </summary>
        /// <param name="position">Value representing the <see cref="StreamPosition"/>.</param>
        public StreamPosition(ulong position) => Value = position;

        /// <summary>
        /// Implicitly convert from <see cref="ulong" /> to <see cref="StreamPosition" />.
        /// </summary>
        /// <param name="position">Position number as <see cref="ulong"/>.</param>
        public static implicit operator StreamPosition(ulong position) => new StreamPosition(position);
    }
}