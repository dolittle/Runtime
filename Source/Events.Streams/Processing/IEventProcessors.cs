// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Defines a system that can process events.
    /// </summary>
    public interface IEventProcessors
    {
        /// <summary>
        /// Processes an event and returns the state.
        /// </summary>
        /// <param name="eventStreamId">Stream id.</param>
        /// <param name="eventEnvelope">Event envelope.</param>
        /// <returns>The processing state.</returns>
        ProcessingState Process(EventStreamId eventStreamId, EventEnvelope eventEnvelope);
    }
}