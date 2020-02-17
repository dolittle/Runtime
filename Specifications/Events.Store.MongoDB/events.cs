// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class events
    {
        public const string some_event_content = "{\"something\": \"text\"}";

        public static BsonDocument some_event_content_bson_document => BsonDocument.Parse("{\"something\": \"text\"}");

        public static Event an_event_from_aggregate(uint event_log_version, uint aggregate_version) =>
            new Event(
                event_log_version,
                metadata.random_event_metadata,
                metadata.random_aggregate_metadata_from_aggregate_event_with_version(aggregate_version),
                false,
                some_event_content_bson_document);

        public static Event an_event_not_from_aggregate(uint event_log_version) =>
            new Event(
                event_log_version,
                metadata.random_event_metadata,
                metadata.aggregate_metadata_from_non_aggregate_event,
                false,
                some_event_content_bson_document);
    }
}