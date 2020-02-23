// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class stream_event_builder
    {
        Events.StreamEvent _instance;

        public stream_event_builder(uint stream_position) =>
            _instance = new Events.StreamEvent(stream_position, metadata.random_stream_event_metadata, metadata.aggregate_metadata_from_non_aggregate_event, events.some_event_content_bson_document);

        public stream_event_builder(uint stream_position, uint aggregate_version) =>
            _instance = new Events.StreamEvent(stream_position, metadata.random_stream_event_metadata, metadata.random_aggregate_metadata_from_aggregate_event_with_version(aggregate_version), events.some_event_content_bson_document);

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
    }
}