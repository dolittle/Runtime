// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class public_event_metadata_builder
    {
        PublicEventMetadata _instance;

        public public_event_metadata_builder() =>
            _instance = new PublicEventMetadata(
                0,
                DateTimeOffset.UtcNow,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                CauseType.Command,
                0,
                Guid.NewGuid(),
                0);

        public PublicEventMetadata build() => _instance;

        public public_event_metadata_builder with_event_log_version(uint event_log_version)
        {
            _instance.EventLogVersion = event_log_version;
            return this;
        }

        public public_event_metadata_builder with_event_source(Guid event_source)
        {
            _instance.EventSource = event_source;
            return this;
        }
    }
}