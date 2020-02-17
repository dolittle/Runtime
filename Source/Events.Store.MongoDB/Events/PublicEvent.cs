// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an event stored in the public events collection the MongoDB event store.
    /// </summary>
    public class PublicEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEvent"/> class.
        /// </summary>
        /// <param name="streamPosition">The position in the stream.</param>
        /// <param name="eventLogVersion">The event log version this event comes from.</param>
        /// <param name="metadata">The The event metadata.</param>
        /// <param name="content">The event content.</param>
        public PublicEvent(uint streamPosition, uint eventLogVersion, EventMetadata metadata, BsonDocument content)
        {
            StreamPosition = streamPosition;
            EventLogVersion = eventLogVersion;
            Metadata = metadata;
            Content = content;
        }

        /// <summary>
        /// Gets or sets stream position.
        /// </summary>
        [BsonId]
        public uint StreamPosition { get; set; }

        /// <summary>
        /// Gets or sets the event log version of the event.
        /// </summary>
        public uint EventLogVersion { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventMetadata"/> containing the platform generated event information.
        /// </summary>
        public EventMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the domain specific event data.
        /// </summary>
        public BsonDocument Content { get; set; }
    }
}