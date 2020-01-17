// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Defines a system that keeps track of the state of event streams.
    /// </summary>
    public interface IEventProcessorStates
    {
        /// <summary>
        /// Gets the current <see cref="StreamState">state</see> of an event stream.
        /// </summary>
        /// <param name="eventStreamId">The<see cref="EventStreamId">id</see> of the event stream.</param>
        /// <returns>The current <see cref="StreamState">event processor state</see>.</returns>
        StreamState Get(EventStreamId eventStreamId);

        /// <summary>
        /// Updates the current <see cref="StreamState">state</see> of an event stream.
        /// </summary>
        /// <param name="eventStreamId">The<see cref="EventStreamId">id</see> of the event stream.</param>
        /// <param name="streamState">The<see cref="StreamState">state</see> that the event stream should be updated with.</param>
        /// <returns>The updated <see cref="StreamState">state</see>.</returns>
        StreamState Update(EventStreamId eventStreamId, StreamState streamState);
    }
}