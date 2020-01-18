// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Defines a system that can manage event streams.
    /// </summary>
    public interface ICanManageEventStreams
    {
        /// <summary>
        /// Gets the state of an event stream.
        /// </summary>
        /// <param name="eventStreamId">The event stream id.</param>
        /// <returns>The current event stream state.</returns>
        EventStreamState GetState(EventStreamId eventStreamId);

        /// <summary>
        /// Updates the state of an event stream.
        /// </summary>
        /// <param name="eventStreamId">The event stream id.</param>
        /// <param name="state">The stream state.</param>
        /// <returns>Updated event stream state.</returns>
        EventStreamState UpdateState(EventStreamId eventStreamId, StreamState state);
    }
}