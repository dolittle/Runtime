// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Reprents the state of an <see cref="ICanProcessStreamOfEvents">event processor</see>.
    /// </summary>
    public class EventProcessorState
    {
        /// <summary>
        /// Gets or sets the <see cref="ProcessingState">state</see>.
        /// </summary>
        /// <value><see cref="ProcessingState"/>.</value>
        public ProcessingState State { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventStreamOffset">offset</see>.
        /// </summary>
        /// <value><see cref="EventStreamOffset"/>.</value>
        public EventStreamOffset Offset { get; set; }
    }
}