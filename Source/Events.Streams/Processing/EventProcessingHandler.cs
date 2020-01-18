// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanHandleEventProcessing{ProcessingResult}"/> that filters an event stream.
    /// </summary>
    public class EventProcessingHandler : ICanHandleEventProcessing<ProcessingResult>
    {
        /// <inheritdoc/>
        public ProcessingResult Process(EventStreamId eventStreamId, EventEnvelope @event)
        {
            return new ProcessingResult { StreamState = StreamState.Ok };
        }
    }
}