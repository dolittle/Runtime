// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

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
        /// <param name="streamProcessorKey">The unique<see cref="StreamProcessorId" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <returns><see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        Task<StreamProcessorState> Get(StreamProcessorId streamProcessorKey);

        /// <summary>
        /// Sets the Offset (last event processed) for this <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorKey">The unique<see cref="StreamProcessorId" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="streamProcessingState">The new<see cref="StreamProcessingState" />of the <see cref="StreamProcessor" />.</param>
        /// <param name="streamPosition">The new<see cref="StreamPosition" />of the <see cref="StreamProcessor"/>.</param>
        /// <returns>>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Set(StreamProcessorId streamProcessorKey, StreamProcessingState streamProcessingState, StreamPosition streamPosition);

        /// <summary>
        /// Sets the Offset (last event processed) for this <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamProcessorKey">The unique<see cref="StreamProcessorId" /> key representing the <see cref="StreamProcessor"/>.</param>
        /// <param name="streamProcessorState">The new<see cref="StreamProcessorState" />of the <see cref="StreamProcessor" />.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Set(StreamProcessorId streamProcessorKey, StreamProcessorState streamProcessorState);
    }
}