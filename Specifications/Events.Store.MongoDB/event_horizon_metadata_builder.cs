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
                3547732730,
                new DateTime(4241561645, DateTimeKind.Utc),
                Guid.Parse("93491243-6602-4a68-a313-fda290a88e3d"));

        public EventHorizonMetadata build() => _instance;
    }
}