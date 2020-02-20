// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an event stored in a stream in the MongoDB event store.
    /// </summary>
    public class StreamEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamEvent"/> class.
        /// </summary>
        /// <param name="streamPosition">The position in the stream.</param>
        /// <param name="metadata">The event metadata.</param>
        /// <param name="aggregate">The aggregate metadata.</param>
        /// <param name="content">The event content.</param>
        public StreamEvent(uint streamPosition, StreamEventMetadata metadata, AggregateMetadata aggregate, BsonDocument content)
        {
            StreamPosition = streamPosition;
            Metadata = metadata;
            Aggregate = aggregate;
            Content = content;
        }

        /// <summary>
        /// Gets or sets stream position.
        /// </summary>
        [BsonId]
        public uint StreamPosition { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="StreamEventMetadata"/> containing the platform generated event information.
        /// </summary>
        public StreamEventMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the event sourcing specific <see cref="AggregateMetadata"/>.
        /// </summary>
        public AggregateMetadata Aggregate { get; set; }

        /// <summary>
        /// Gets or sets the domain specific event data.
        /// </summary>
        public BsonDocument Content { get; set; }
    }
}