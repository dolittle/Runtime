// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a combination of <see cref="StreamPosition" /> and <see cref="StreamProcessingState" /> that represents the state of an <see cref="StreamProcessor" />.
    /// </summary>
    public class StreamProcessorState : Value<StreamProcessorState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorState"/> class.
        /// </summary>
        /// <param name="streamProcessingState"><see cref="StreamProcessingState"/> state of the processing.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition"/>position of the stream.</param>
        public StreamProcessorState(StreamProcessingState streamProcessingState, StreamPosition streamPosition)
        {
            State = streamProcessingState;
            Position = streamPosition;
        }

        /// <summary>
        /// Gets a new, initial, <see cref="StreamProcessorState" />.
        /// </summary>
        public static StreamProcessorState New => new StreamProcessorState(StreamProcessingState.Waiting, StreamPosition.Start);

        /// <summary>
        /// Gets the <see cref="StreamProcessingState" />.
        /// </summary>
        public StreamProcessingState State { get; }

        /// <summary>
        /// Gets the <see cref="StreamPosition" />.
        /// </summary>
        public StreamPosition Position { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"({State}, {Position.Value})";
        }
    }
}