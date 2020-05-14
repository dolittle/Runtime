// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class event_metadata_builder
    {
        EventMetadata _instance;

        public event_metadata_builder() =>
            _instance = new EventMetadata(
                DateTimeOffset.UtcNow,
                Guid.NewGuid(),
                Guid.NewGuid(),
                0,
                false,
                false,
                0);

        public EventMetadata build() => _instance;

        public event_metadata_builder with_event_source(Guid event_source)
        {
            _instance.EventSource = event_source;
            return this;
        }
    }
}