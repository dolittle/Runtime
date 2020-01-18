// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Defines a system that can manage an event stream.
    /// </summary>
    public interface ICanManageEventStream
    {
        /// <summary>
        /// Gets the state of an event stream.
        /// </summary>
        /// <returns>The current event stream state.</returns>
        EventStreamState GetState();

        /// <summary>
        /// Updates the state of an event stream.
        /// </summary>
        /// <param name="state">The stream state.</param>
        /// <returns>Updated event stream state.</returns>
        EventStreamState UpdateState(StreamState state);
    }
}