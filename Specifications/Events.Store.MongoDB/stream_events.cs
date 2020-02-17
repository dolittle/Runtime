// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class stream_events
    {
        public static MongoDB.Events.StreamEvent an_event_from_aggregate(uint position, uint event_log_version, uint aggregate_version) =>
            new MongoDB.Events.StreamEvent(
                position,
                event_log_version,
                Guid.NewGuid(),
                metadata.random_event_metadata,
                metadata.random_aggregate_metadata_from_aggregate_event_with_version(aggregate_version),
                false,
                events.some_event_content_bson_document);

        public static MongoDB.Events.StreamEvent an_event_not_from_aggregate(uint position, uint event_log_version) => an_event_not_from_aggregate_with_partition(position, event_log_version, Guid.NewGuid());

        public static MongoDB.Events.StreamEvent an_event_not_from_aggregate_with_partition(uint position, uint event_log_version, PartitionId partition) =>
            new MongoDB.Events.StreamEvent(
                position,
                event_log_version,
                partition,
                metadata.random_event_metadata,
                metadata.aggregate_metadata_from_non_aggregate_event,
                false,
                events.some_event_content_bson_document);
    }
}