// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class metadata
    {
        public static EventMetadata random_event_metadata =>
            new EventMetadata(
                DateTimeOffset.UtcNow,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                CauseType.Command,
                0,
                Guid.NewGuid(),
                1);

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