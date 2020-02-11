// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents the stream id and stream position of a <see cref="StreamEvent" />.
    /// </summary>
    public class StreamIdAndPosition : IEquatable<StreamIdAndPosition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamIdAndPosition"/> class.
        /// </summary>
        /// <param name="streamId">The stream id.</param>
        /// <param name="position">The position.</param>
        public StreamIdAndPosition(Guid streamId, uint position)
        {
            StreamId = streamId;
            Position = position;
        }

        /// <summary>
        /// Gets or sets the stream id.
        /// </summary>
        public Guid StreamId { get; set; }

        /// <summary>
        /// Gets or sets stream position.
        /// </summary>
        public uint Position { get; set; }

        /// <inheritdoc/>
        public bool Equals(StreamIdAndPosition other) => Position == other.Position && StreamId.Equals(other.StreamId);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as StreamIdAndPosition);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(StreamId, Position);
    }
}