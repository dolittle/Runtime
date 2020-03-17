// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents a <see cref="StreamPosition" /> range.
    /// </summary>
    public class StreamPositionRange : Value<StreamPositionRange>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamPositionRange"/> class.
        /// </summary>
        /// <param name="from">The from <see cref="StreamPosition" />.</param>
        /// <param name="to">The to <see cref="StreamPosition" />.</param>
        public StreamPositionRange(StreamPosition from, StreamPosition to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// Gets the, inclusive, starting point of the range.
        /// </summary>
        public StreamPosition From { get; }

        /// <summary>
        /// Gets the, inclusive, end of the range.
        /// </summary>
        public StreamPosition To { get; }

        /// <inheritdoc/>
        public override string ToString() => $"({From}, {To})";
    }
}