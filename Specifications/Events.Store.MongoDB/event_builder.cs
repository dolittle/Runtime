// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB;

public class event_builder
{
    MongoDB.Events.Event _instance;

    public event_builder(EventLogSequenceNumber event_log_sequence_number) =>
        _instance = new MongoDB.Events.Event(
            event_log_sequence_number,
            execution_contexts.create_store(),
            metadata.random_event_metadata,
            metadata.aggregate_metadata_from_non_aggregate_event,
            null,
            events.some_event_content_bson_document);

    public event_builder(EventLogSequenceNumber event_log_sequence_number, AggregateRootVersion aggregate_version) =>
        _instance = new MongoDB.Events.Event(
            event_log_sequence_number,
            execution_contexts.create_store(),
            metadata.random_event_metadata,
            metadata.random_aggregate_metadata_from_aggregate_event_with_version(aggregate_version),
            null,
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
        _instance.EventHorizon.Consent = Guid.Parse("df838974-100b-4a07-9e44-08e2c7d7e99a");
        _instance.EventHorizon.ExternalEventLogSequenceNumber = 71883084;
        _instance.EventHorizon.Received = new DateTime(2944480155, DateTimeKind.Utc);
        return this;
    }
}