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
        /// Gets or sets the <see cref="StreamState">state</see>.
        /// </summary>
        public StreamState StreamState { get; set; }

        /// <summary>
        /// Gets or sets  the <see cref="EventStreamOffset">offset</see>.
        /// </summary>
        /// <value><see cref="EventStreamOffset"/>.</value>
        public EventStreamOffset Offset { get; set; } = EventStreamOffset.Start;
    }
}