// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanHandleEventProcessing{FilteringResult}"/> that filters an event stream.
    /// </summary>
    public class EventStreamFilterHandler : ICanHandleEventProcessing<FilteringResult>
    {
        /// <inheritdoc/>
        public FilteringResult Process(EventStreamId eventStreamId, EventEnvelope @event)
        {
            return new FilteringResult { StreamState = StreamState.Ok, IncludeEvent = true, Partition = 0 };
        }
    }
}