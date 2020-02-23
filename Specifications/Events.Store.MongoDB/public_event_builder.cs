// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class public_event_builder
    {
        PublicEvent _instance;

        public public_event_builder(uint stream_position) =>
            _instance = new PublicEvent(stream_position, metadata.random_public_event_metadata, events.some_event_content_bson_document);

        public PublicEvent build() => _instance;

        public public_event_builder with_metadata(PublicEventMetadata metadata)
        {
            _instance.Metadata = metadata;
            return this;
        }

        public public_event_builder with_content(string content) => with_content(BsonDocument.Parse(content));

        public public_event_builder with_content(BsonDocument document)
        {
            _instance.Content = document;
            return this;
        }
    }
}