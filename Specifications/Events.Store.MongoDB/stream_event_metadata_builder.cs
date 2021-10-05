// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class stream_event_metadata_builder
    {
        StreamEventMetadata _instance;

        public stream_event_metadata_builder() =>
            _instance = new StreamEventMetadata(
                59077,
                new DateTime(2943653239, DateTimeKind.Utc),
                "the event sourec",
                Guid.Parse("a4ca4bbd-21d3-4caf-9e5e-b46ce26b0b2e"),
                62012,
                false);

        public StreamEventMetadata build() => _instance;

        public stream_event_metadata_builder with_event_log_sequence_number(uint event_log_sequence_number)
        {
            _instance.EventLogSequenceNumber = event_log_sequence_number;
            return this;
        }

        public stream_event_metadata_builder with_event_source(string event_source)
        {
            _instance.EventSource = event_source;
            return this;
        }
    }
}