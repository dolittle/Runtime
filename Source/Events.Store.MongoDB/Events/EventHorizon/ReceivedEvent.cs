// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
     /// <summary>
    /// Represents an event stored in the received events from microservice collection in the MongoDB event store.
    /// </summary>
    public class ReceivedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedEvent"/> class.
        /// </summary>
        /// <param name="streamPosition">The position in the stream.</param>
        /// <param name="metadata">The event metadata.</param>
        /// <param name="content">The event content.</param>
        public ReceivedEvent(uint streamPosition, ReceivedEventMetadata metadata, BsonDocument content)
        {
            StreamPosition = streamPosition;
            Metadata = metadata;
            Content = content;
        }

        /// <summary>
        /// Gets or sets stream position.
        /// </summary>
        [BsonId]
        public uint StreamPosition { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ReceivedEventMetadata"/> containing the platform generated event information.
        /// </summary>
        public ReceivedEventMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the domain specific event data.
        /// </summary>
        public BsonDocument Content { get; set; }
    }
}