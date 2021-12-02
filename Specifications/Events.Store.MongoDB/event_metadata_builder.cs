// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;

namespace Dolittle.Runtime.Events.Store.MongoDB;

public class event_metadata_builder
{
    EventMetadata _instance;

    public event_metadata_builder() =>
        _instance = new EventMetadata(
            new DateTime(214800287, DateTimeKind.Utc),
            "0bc15693-6141-eventsource-11a610ec7c82",
            Guid.Parse("17398e13-539e-4628-b419-ee081b631c57"),
            1990762078,
            false);

    public EventMetadata build() => _instance;

    public event_metadata_builder with_event_source(string event_source)
    {
        _instance.EventSource = event_source;
        return this;
    }
}