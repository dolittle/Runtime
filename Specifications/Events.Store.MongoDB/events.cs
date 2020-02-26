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

        public static event_builder new_event(uint event_log_sequence_number, uint aggregate_version) => new event_builder(event_log_sequence_number, aggregate_version);

        public static event_builder new_event_not_from_aggregate(uint event_log_sequence_number) => new event_builder(event_log_sequence_number);

        public static Event an_event(uint event_log_sequence_number, uint aggregate_version) => new event_builder(event_log_sequence_number, aggregate_version).build();

        public static Event an_event_not_from_aggregate(uint event_log_sequence_number) => new event_builder(event_log_sequence_number).build();

        public static stream_event_builder new_stream_event(uint stream_position, uint aggregate_version) => new stream_event_builder(stream_position, aggregate_version);

        public static stream_event_builder new_stream_event_not_from_aggregate(uint stream_position) => new stream_event_builder(stream_position);

        public static Events.StreamEvent a_stream_event(uint stream_position, uint aggregate_version) => new stream_event_builder(stream_position, aggregate_version).build();

        public static Events.StreamEvent a_stream_event_not_from_aggregate(uint stream_position) => new stream_event_builder(stream_position).build();

        public static public_event_builder new_public_event(uint stream_position) => new public_event_builder(stream_position);

        public static PublicEvent a_public_event(uint stream_position) => new public_event_builder(stream_position).build();
    }
}