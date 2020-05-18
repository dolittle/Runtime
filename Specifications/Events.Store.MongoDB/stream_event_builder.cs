// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class stream_event_builder
    {
        Events.StreamEvent _instance;

        public stream_event_builder(uint stream_position, Guid partition) =>
            _instance = new Events.StreamEvent(
                stream_position,
                partition,
                execution_contexts.create_store(),
                metadata.random_stream_event_metadata,
                metadata.aggregate_metadata_from_non_aggregate_event,
                new EventHorizonMetadata(),
                events.some_event_content_bson_document);

        public stream_event_builder(uint stream_position, Guid partition, uint aggregate_version) =>
            _instance = new Events.StreamEvent(
                stream_position,
                partition,
                execution_contexts.create_store(),
                metadata.random_stream_event_metadata,
                metadata.random_aggregate_metadata_from_aggregate_event_with_version(aggregate_version),
                new EventHorizonMetadata(),
                events.some_event_content_bson_document);

        public Events.StreamEvent build() => _instance;

        public stream_event_builder with_metadata(StreamEventMetadata metadata)
        {
            _instance.Metadata = metadata;
            return this;
        }

        public stream_event_builder with_aggregate_metadata(AggregateMetadata metadata)
        {
            _instance.Aggregate = metadata;
            return this;
        }

        public stream_event_builder with_content(string content) => with_content(BsonDocument.Parse(content));

        public stream_event_builder with_content(BsonDocument document)
        {
            _instance.Content = document;
            return this;
        }

        public stream_event_builder with_partition(PartitionId partition)
        {
            _instance.Partition = partition;
            return this;
        }

        public stream_event_builder with_event_log_sequence_number(EventLogSequenceNumber event_log_sequence_number)
        {
            _instance.Metadata.EventLogSequenceNumber = event_log_sequence_number;
            return this;
        }

        public stream_event_builder from_event_horizon()
        {
            _instance.EventHorizonMetadata.FromEventHorizon = true;
            _instance.EventHorizonMetadata.Consent = Guid.NewGuid();
            _instance.EventHorizonMetadata.ExternalEventLogSequenceNumber = (ulong)new Random().Next();
            _instance.EventHorizonMetadata.Received = DateTimeOffset.UtcNow;
            return this;
        }
    }
}