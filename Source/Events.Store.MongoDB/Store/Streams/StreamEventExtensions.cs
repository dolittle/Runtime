// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.MongoDB.EventLog;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Extension methods for <see cref="StreamEvent" />.
    /// </summary>
    public static class StreamEventExtensions
    {
        /// <summary>
        /// Converts a <see cref="CommittedEvent" /> to a <see cref="StreamEvent" />.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent" /> to convert.</param>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <returns>The converted <see cref="StreamEvent" />.</returns>
        public static StreamEvent ToStreamEvent(this CommittedEvent committedEvent, StreamId streamId, StreamPosition streamPosition, PartitionId partitionId) =>
            new StreamEvent(
                streamId,
                streamPosition,
                committedEvent.EventLogVersion,
                partitionId,
                committedEvent.GetEventMetadata(),
                committedEvent.GetAggregateMetadata(),
                committedEvent.Content);

        /// <summary>
        /// Converts the <see cref="StreamEvent" /> to <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="streamEvent">The <see cref="StreamEvent" />.</param>
        /// <returns>The converted <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this StreamEvent streamEvent)
        {
            var committedEvent = new CommittedEvent(
                streamEvent.EventLogVersion,
                streamEvent.Metadata.Occurred,
                streamEvent.Metadata.Correlation,
                streamEvent.Metadata.Microservice,
                streamEvent.Metadata.Tenant,
                new Cause(streamEvent.Metadata.CauseType, streamEvent.Metadata.CausePosition),
                new Artifacts.Artifact(streamEvent.Metadata.TypeId, streamEvent.Metadata.TypeGeneration),
                streamEvent.Content.ToString());

            return committedEvent;
        }

        /// <summary>
        /// Converts the <see cref="StreamEvent" /> to <see cref="CommittedEventWithPartition" />.
        /// </summary>
        /// <param name="streamEvent">The <see cref="StreamEvent" />.</param>
        /// <param name="partitionId">The <see cref="PartitionId" />.</param>
        /// <returns>The converted <see cref="CommittedEventWithPartition" />.</returns>
        public static CommittedEventWithPartition ToCommittedEventWithPartition(this StreamEvent streamEvent, PartitionId partitionId) =>
            new CommittedEventWithPartition(streamEvent.ToCommittedEvent(), partitionId);
    }
}