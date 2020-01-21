// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a unique key for a <see cref="StreamProcessor" />.
    /// </summary>
    public class StreamProcessorKey : Value<StreamProcessorKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorKey"/> class.
        /// </summary>
        /// <param name="eventProcessorId"><see cref="EventProcessorId"/>.</param>
        /// <param name="sourceStreamId">The <see cref="EventProcessorId"/>.</param>
        public StreamProcessorKey(EventProcessorId eventProcessorId, StreamId sourceStreamId)
        {
            EventProcessorId = eventProcessorId;
            SourceStreamId = sourceStreamId;
        }

        /// <summary>
        /// Gets the <see cref="EventProcessorId" />.
        /// </summary>
        public EventProcessorId EventProcessorId { get; }

        /// <summary>
        /// Gets the <see cref="StreamId" />.
        /// </summary>
        public StreamId SourceStreamId { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{EventProcessorId} - {SourceStreamId}";
        }
    }
}