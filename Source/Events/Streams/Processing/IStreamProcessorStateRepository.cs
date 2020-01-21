// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Defines how we get and set the <see cref="StreamProcessorState"/>for <see cref="StreamProcessor" >stream processors</see>.
    /// </summary>
    public interface IStreamProcessorStateRepository : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="StreamProcessorState" /> for this <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="eventProcessorId"><see cref="EventProcessorId" />of the <see cref="IEventProcessor" /> associated with this <see cref="StreamProcessor" />.</param>
        /// <returns><see cref="StreamProcessorState" />for this <see cref="StreamProcessor" />.</returns>
        StreamProcessorState Get(EventProcessorId eventProcessorId);

        /// <summary>
        /// Sets the Offset (last event processed) for this <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="eventProcessorId"><see cref="EventProcessorId" />of the <see cref="IEventProcessor" /> associated with this <see cref="StreamProcessor" />.</param>
        /// <param name="streamProcessingState">The new<see cref="StreamProcessingState" />of the <see cref="StreamProcessor" />.</param>
        /// <param name="streamPosition">The new<see cref="StreamPosition" />of the <see cref="StreamProcessor"/>.</param>
        void Set(EventProcessorId eventProcessorId, StreamProcessingState streamProcessingState, StreamPosition streamPosition);
    }
}