// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Represents a combination of the <see cref="StreamPosition" /> and a <see cref="StreamProcessorState" /> that represents the state of an <see cref="StreamProcessor" />.
    /// </summary>
    public class StreamProcessorStateAndPosition : Value<StreamProcessorStateAndPosition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorStateAndPosition"/> class.
        /// </summary>
        /// <param name="streamProcessorState"><see cref="StreamProcessorState"/> state of the stream processor.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition"/>position of the stream.</param>
        public StreamProcessorStateAndPosition(StreamProcessorState streamProcessorState, StreamPosition streamPosition)
        {
            State = streamProcessorState;
            Position = streamPosition;
        }

        /// <summary>
        /// Gets a new, initial, <see cref="StreamProcessorStateAndPosition" />.
        /// </summary>
        public static StreamProcessorStateAndPosition New => new StreamProcessorStateAndPosition(StreamProcessorState.Running, StreamPosition.Start);

        /// <summary>
        /// Gets the <see cref="StreamProcessorState" />state of the <see cref="StreamProcessor" />.
        /// </summary>
        public StreamProcessorState State { get; }

        /// <summary>
        /// Gets the <see cref="StreamPosition" />of the <see cref="StreamProcessor" />.
        /// </summary>
        public StreamPosition Position { get; }
    }
}