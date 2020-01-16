// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessors"/>.
    /// </summary>
    public class EventProcessors : IEventProcessors
    {
        /// <inheritdoc/>
        public ProcessingState Process(EventStreamId eventStreamId, EventEnvelope eventEnvelope)
        {
            return ProcessingState.Ok;
        }
    }
}