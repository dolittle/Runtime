// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class events
    {
        public static string some_event_content => "{\"something\": \"text\"}";

        public static BsonDocument some_event_content_bson_document => BsonDocument.Parse("{\"something\": \"text\"}");

        public static Event random_event_from_aggregate_with_position_and_event_log_version(uint position, uint event_log_version, uint aggregate_version) =>
            new Event(
                position,
                event_log_version,
                Guid.NewGuid(),
                metadata.random_event_metadata,
                metadata.random_aggregate_metadata_from_aggregate_event_with_version(aggregate_version),
                some_event_content_bson_document);

        public static Event random_event_not_from_aggregate_with_position_and_event_log_version(uint position, uint event_log_version) =>
            new Event(
                position,
                event_log_version,
                Guid.NewGuid(),
                metadata.random_event_metadata,
                metadata.aggregate_metadata_from_non_aggregate_event,
                some_event_content_bson_document);
    }
}