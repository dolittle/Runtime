// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store.Streams
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
        /// <param name="length">Wanted length of the range moving forward.</param>
        public StreamPositionRange(StreamPosition from, ulong length)
        {
            From = from;
            Length = length;
        }

        /// <summary>
        /// Gets the, inclusive, starting point of the range.
        /// </summary>
        public StreamPosition From { get; }

        /// <summary>
        /// Gets the length of the wanted range.
        /// </summary>
        public ulong Length { get; }

        /// <inheritdoc/>
        public override string ToString() => $"({From}, {From + Length})";
    }
}