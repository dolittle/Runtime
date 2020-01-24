// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines how we get and set the <see cref="StreamProcessorState"/>for <see cref="StreamProcessor" >stream processors</see>.
    /// </summary>
    public interface IStreamProcessorStateRepository : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="StreamProcessorState" /> for this <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorKey">The unique<see cref="StreamProcessorKey" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <returns><see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        StreamProcessorState Get(StreamProcessorKey streamProcessorKey);

        /// <summary>
        /// Sets the Offset (last event processed) for this <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorKey">The unique<see cref="StreamProcessorKey" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="streamProcessingState">The new<see cref="StreamProcessingState" />of the <see cref="StreamProcessor" />.</param>
        /// <param name="streamPosition">The new<see cref="StreamPosition" />of the <see cref="StreamProcessor"/>.</param>
        void Set(StreamProcessorKey streamProcessorKey, StreamProcessingState streamProcessingState, StreamPosition streamPosition);

        /// <summary>
        /// Sets the Offset (last event processed) for this <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorKey">The unique<see cref="StreamProcessorKey" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="streamProcessorState">The new<see cref="StreamProcessorState" />of the <see cref="StreamProcessor" />.</param>
        void Set(StreamProcessorKey streamProcessorKey, StreamProcessorState streamProcessorState);
    }
}