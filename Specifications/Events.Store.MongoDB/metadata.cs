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

        public static event_horizon_metadata_builder new_event_horizon_metadata => new event_horizon_metadata_builder();

        public static EventHorizonMetadata random_event_horizon_metadata => new event_horizon_metadata_builder().build();

        public static AggregateMetadata random_aggregate_metadata_from_aggregate_event_with_version(AggregateRootVersion version) =>
            new AggregateMetadata(
                true,
                Guid.Parse("31e26598-838b-47b1-82e8-a4f8e7085ff8"),
                51434,
                version);

        public static AggregateMetadata aggregate_metadata_from_non_aggregate_event =>
            new AggregateMetadata();
    }
}