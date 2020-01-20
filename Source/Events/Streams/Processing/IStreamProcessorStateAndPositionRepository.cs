// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Defines how we get and set the <see cref="StreamProcessorState">stream processor state</see> <see cref="StreamPosition">stream position</see> of a <see cref="StreamProcessor">stream processor</see>.
    /// </summary>
    public interface IStreamProcessorStateAndPositionRepository : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="StreamProcessorStateAndPosition">state and position</see> for this <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="eventProcessorId"><see cref="EventProcessorId">id</see> of the <see cref="IEventProcessor" /> associated with this <see cref="StreamProcessor" />.</param>
        /// <returns><see cref="StreamProcessorStateAndPosition">state and position</see> for this <see cref="StreamProcessor" />.</returns>
        StreamProcessorStateAndPosition Get(EventProcessorId eventProcessorId);

        /// <summary>
        /// Sets the Offset (last event processed) for this <see cref="IEventProcessor" />.
        /// </summary>
        /// <param name="eventProcessorId"><see cref="EventProcessorId">Id</see> of the <see cref="IEventProcessor" /> associated with this <see cref="StreamProcessor" />.</param>
        /// <param name="streamProcessorState"><see cref="StreamProcessorState">state</see> of the <see cref="StreamProcessor" />.</param>
        /// <param name="streamPosition"><see cref="StreamPosition">stream position</see>of the <see cref="StreamProcessor"/>.</param>
        void Set(EventProcessorId eventProcessorId, StreamProcessorState streamProcessorState, StreamPosition streamPosition);
    }
}