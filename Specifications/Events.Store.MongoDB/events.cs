// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class events
    {
        public const string some_event_content = "{ \"something\" : \"text\" }";

        public static BsonDocument some_event_content_bson_document => BsonDocument.Parse("{\"something\": \"text\"}");

        public static event_builder new_event(EventLogSequenceNumber event_log_sequence_number, AggregateRootVersion aggregate_version) => new event_builder(event_log_sequence_number, aggregate_version);

        public static event_builder new_event_not_from_aggregate(EventLogSequenceNumber event_log_sequence_number) => new event_builder(event_log_sequence_number);

        public static MongoDB.Events.Event an_event(EventLogSequenceNumber event_log_sequence_number, AggregateRootVersion aggregate_version) => new event_builder(event_log_sequence_number, aggregate_version).build();

        public static MongoDB.Events.Event an_external_event(EventLogSequenceNumber event_log_sequence_number) => new event_builder(event_log_sequence_number).from_event_horizon().build();

        public static MongoDB.Events.Event an_event_not_from_aggregate(EventLogSequenceNumber event_log_sequence_number) => new event_builder(event_log_sequence_number).build();

        public static stream_event_builder new_stream_event(StreamPosition stream_position, PartitionId partition, AggregateRootVersion aggregate_version) => new stream_event_builder(stream_position, partition, aggregate_version);

        public static stream_event_builder new_stream_event_not_from_aggregate(StreamPosition stream_position, PartitionId partition) => new stream_event_builder(stream_position, partition);

        public static MongoDB.Events.StreamEvent a_stream_event(StreamPosition stream_position, PartitionId partition, AggregateRootVersion aggregate_version) => new stream_event_builder(stream_position, partition, aggregate_version).build();

        public static MongoDB.Events.StreamEvent an_external_stream_event(StreamPosition stream_position, PartitionId partition) => new stream_event_builder(stream_position, partition).from_event_horizon().build();

        public static MongoDB.Events.StreamEvent a_stream_event_not_from_aggregate(StreamPosition stream_position, PartitionId partition) => new stream_event_builder(stream_position, partition).build();
    }
}