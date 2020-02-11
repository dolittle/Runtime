// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.EventLog;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Represents an event stored in a stream.
    /// </summary>
    public class StreamEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamEvent"/> class.
        /// </summary>
        /// <param name="streamId">The stream id.</param>
        /// <param name="position">The stream position.</param>
        /// <param name="eventLogVersion">The event log version.</param>
        /// <param name="partitionId">The partition id.</param>
        /// <param name="metadata">The event metadata.</param>
        /// <param name="aggregate">The event aggregate metadata.</param>
        /// <param name="content">The content.</param>
        public StreamEvent(
            Guid streamId,
            uint position,
            uint eventLogVersion,
            Guid partitionId,
            EventMetadata metadata,
            AggregateMetadata aggregate,
            string content)
        {
            StreamIdAndPosition = new StreamIdAndPosition(streamId, position);
            EventLogVersion = eventLogVersion;
            PartitionId = partitionId;
            Metadata = metadata;
            Aggregate = aggregate;
            Content = BsonDocument.Parse(content);
        }

        /// <summary>
        /// Gets or sets the stream id and position.
        /// </summary>
        [BsonId]
        public StreamIdAndPosition StreamIdAndPosition { get; set; }

        /// <summary>
        /// Gets or sets the event log version.
        /// </summary>
        public uint EventLogVersion { get; set; }

        /// <summary>
        /// Gets or sets the partition id.
        /// </summary>
        public Guid PartitionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventMetadata"/> containing the platform generated event information.
        /// </summary>
        public EventMetadata Metadata { get; set; }

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