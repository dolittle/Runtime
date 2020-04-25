// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class metadata
    {
        public static event_metadata_builder new_event_metadata => new event_metadata_builder();

        public static EventMetadata random_event_metadata => new event_metadata_builder().build();

        public static stream_event_metadata_builder new_stream_event_metadata => new stream_event_metadata_builder();

        public static StreamEventMetadata random_stream_event_metadata => new stream_event_metadata_builder().build();

        public static AggregateMetadata random_aggregate_metadata_from_aggregate_event_with_version(uint version) =>
            new AggregateMetadata(
                true,
                Guid.NewGuid(),
                1,
                version);

        public static AggregateMetadata aggregate_metadata_from_non_aggregate_event =>
            new AggregateMetadata();
    }
}