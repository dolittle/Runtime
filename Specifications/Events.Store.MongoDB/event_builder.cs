// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class event_builder
    {
        Event _instance;

        public event_builder(uint event_log_version) =>
            _instance = new Event(event_log_version, metadata.random_event_metadata, metadata.aggregate_metadata_from_non_aggregate_event, events.some_event_content_bson_document);

        public event_builder(uint event_log_version, uint aggregate_version) =>
            _instance = new Event(event_log_version, metadata.random_event_metadata, metadata.random_aggregate_metadata_from_aggregate_event_with_version(aggregate_version), events.some_event_content_bson_document);

        public Event build() => _instance;

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
    }
}