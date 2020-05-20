// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class event_builder
    {
        MongoDB.Events.Event _instance;

        public event_builder(uint event_log_sequence_number) =>
            _instance = new MongoDB.Events.Event(
                event_log_sequence_number,
                execution_contexts.create_store(),
                metadata.random_event_metadata,
                metadata.aggregate_metadata_from_non_aggregate_event,
                new EventHorizonMetadata(),
                events.some_event_content_bson_document);

        public event_builder(uint event_log_sequence_number, uint aggregate_version) =>
            _instance = new MongoDB.Events.Event(
                event_log_sequence_number,
                execution_contexts.create_store(),
                metadata.random_event_metadata,
                metadata.random_aggregate_metadata_from_aggregate_event_with_version(aggregate_version),
                new EventHorizonMetadata(),
                events.some_event_content_bson_document);

        public MongoDB.Events.Event build() => _instance;

        public event_builder with_metadata(EventMetadata metadata)
        {
            _instance.Metadata = metadata;
            return this;
        }

        public event_builder with_aggregate_metadata(AggregateMetadata metadata)
        {
            _instance.Aggregate = metadata;
            return this;
        }

        public event_builder with_content(string content) => with_content(BsonDocument.Parse(content));

        public event_builder with_content(BsonDocument document)
        {
            _instance.Content = document;
            return this;
        }

        public event_builder from_event_horizon()
        {
            _instance.EventHorizon.FromEventHorizon = true;
            _instance.EventHorizon.Consent = Guid.NewGuid();
            _instance.EventHorizon.ExternalEventLogSequenceNumber = (ulong)new Random().Next();
            _instance.EventHorizon.Received = DateTime.UtcNow;
            return this;
        }
    }
}