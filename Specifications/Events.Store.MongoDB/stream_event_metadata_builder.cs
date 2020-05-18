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
                0,
                DateTimeOffset.UtcNow,
                Guid.NewGuid(),
                Guid.NewGuid(),
                0,
                false);

        public StreamEventMetadata build() => _instance;

        public stream_event_metadata_builder with_event_log_sequence_number(uint event_log_sequence_number)
        {
            _instance.EventLogSequenceNumber = event_log_sequence_number;
            return this;
        }

        public stream_event_metadata_builder with_event_source(Guid event_source)
        {
            _instance.EventSource = event_source;
            return this;
        }
    }
}