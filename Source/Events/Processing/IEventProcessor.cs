// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines something that is capable of processing an event.
    /// </summary>
    public interface IEventProcessor
    {
        /// <summary>
        /// Gets the identifier for the <see cref="IEventProcessor"/>.
        /// </summary>
        EventProcessorId Identifier { get; }

        /// <summary>
        /// Gets the <see cref="StreamId">stream id</see>.
        /// </summary>
        StreamId StreamId { get; }

        /// <summary>
        /// Process an event.
        /// </summary>
        /// <param name="eventEnvelope"><see cref="CommittedEventEnvelope"/> for event to process.</param>
        /// <returns><see cref="ProcessingResult">Processing result</see>.</returns>
        ProcessingResult Process(CommittedEventEnvelope eventEnvelope);
    }
}