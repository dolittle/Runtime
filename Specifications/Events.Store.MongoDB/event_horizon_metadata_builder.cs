// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class event_horizon_metadata_builder
    {
        EventHorizonMetadata _instance;

        public event_horizon_metadata_builder() =>
            _instance = new EventHorizonMetadata(
                0,
                DateTime.UtcNow,
                Guid.Empty);

        public EventHorizonMetadata build() => _instance;

        public event_horizon_metadata_builder with_external_event_log_sequence_number(uint event_log_sequence_number)
        {
            _instance.ExternalEventLogSequenceNumber = event_log_sequence_number;
            return this;
        }
    }
}