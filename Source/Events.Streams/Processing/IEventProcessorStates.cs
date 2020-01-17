// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Defines a system that keeps track of the state of event processors.
    /// </summary>
    public interface IEventProcessorStates
    {
        /// <summary>
        /// Gets the current <see cref="EventProcessorState">state</see> of an event processor.
        /// </summary>
        /// <param name="eventStreamId">The<see cref="EventStreamId">id</see> of the event processor.</param>
        /// <returns>The current <see cref="EventProcessorState">event processor state</see>.</returns>
        EventProcessorState Get(EventStreamId eventStreamId);

        /// <summary>
        /// Updates the current <see cref="EventProcessorState">state</see> of an event processor.
        /// </summary>
        /// <param name="eventStreamId">The<see cref="EventStreamId">id</see> of the event processor.</param>
        /// <param name="processingState">The<see cref="ProcessingState">state</see> that the event processor should be updated with.</param>
        /// <returns>The updated <see cref="EventProcessorState">event processor state</see>.</returns>
        EventProcessorState Update(EventStreamId eventStreamId, ProcessingState processingState);
    }
}