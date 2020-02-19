// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
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
        /// <param name="eventLogVersion">The event log version this event comes from.</param>
        /// <param name="metadata">The event metadata.</param>
        /// <param name="fromTenant">The tenant that produced this event.</param>
        /// <param name="content">The event content.</param>
        public ReceivedEvent(uint streamPosition, uint eventLogVersion, EventMetadata metadata, Guid fromTenant, BsonDocument content)
        {
            StreamPosition = streamPosition;
            EventLogVersion = eventLogVersion;
            Metadata = metadata;
            FromTenant = fromTenant;
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
        /// Gets or sets the tenant that this Event comes from.
        /// </summary>
        public Guid FromTenant { get; set; }

        /// <summary>
        /// Gets or sets the domain specific event data.
        /// </summary>
        public BsonDocument Content { get; set; }
    }
}