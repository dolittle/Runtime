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
        /// A null-state of <see cref="EventProcessorState"/>.
        /// </summary>
        public static readonly EventProcessorState NullState = new EventProcessorState { Offset = 0, ProcessingState = ProcessingState.NullState };

        /// <summary>
        /// Gets or sets the <see cref="ProcessingState">state</see>.
        /// </summary>
        /// <value><see cref="Streams.ProcessingState"/>.</value>
        public ProcessingState ProcessingState { get; set; }

        /// <summary>
        /// Gets or sets  the <see cref="EventStreamOffset">offset</see>.
        /// </summary>
        /// <value><see cref="EventStreamOffset"/>.</value>
        public EventStreamOffset Offset { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EventProcessorState">state</see> is a null state.
        /// </summary>
        /// <returns>Whether or not this is a null state.</returns>
        public bool IsNullState => ProcessingState == ProcessingState.NullState;
    }
}