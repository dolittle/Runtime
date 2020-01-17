// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Reprents the state of an event processor.
    /// </summary>
    public class EventStreamState
    {
        /// <summary>
        /// A null-state of <see cref="EventStreamState"/>.
        /// </summary>
        public static readonly EventStreamState NullState = new EventStreamState { Offset = 0, StreamState = StreamState.NullState };

        /// <summary>
        /// Gets or sets the <see cref="StreamState">state</see>.
        /// </summary>
        public StreamState StreamState { get; set; } = StreamState.NullState;

        /// <summary>
        /// Gets or sets  the <see cref="EventStreamOffset">offset</see>.
        /// </summary>
        /// <value><see cref="EventStreamOffset"/>.</value>
        public EventStreamOffset Offset { get; set; } = EventStreamOffset.Start;

        /// <summary>
        /// Gets a value indicating whether this <see cref="EventStreamState">state</see> is a null state.
        /// </summary>
        /// <returns>Whether or not this is a null state.</returns>
        public bool IsNullState => StreamState == StreamState.NullState;
    }
}