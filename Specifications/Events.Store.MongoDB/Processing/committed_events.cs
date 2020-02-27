// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class committed_events
    {
        public static CommittedAggregateEvent a_committed_aggregate_event(EventLogVersion event_log_version, ArtifactId aggregate, EventSourceId event_source, AggregateRootVersion aggregate_root_version) =>
            new CommittedAggregateEvent(
                new Artifact(aggregate, 0),
                aggregate_root_version,
                event_log_version,
                DateTimeOffset.UtcNow,
                event_source,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                new Artifact(Guid.NewGuid(), 1),
                false,
                events.some_event_content);

        public static CommittedEvent a_committed_event(EventLogVersion event_log_version) =>
            new CommittedEvent(
                event_log_version,
                DateTimeOffset.UtcNow,
                EventSourceId.NotSet,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                new Artifact(Guid.NewGuid(), 1),
                false,
                events.some_event_content);

        public static CommittedAggregateEvent a_committed_aggregate_event_with_type(EventLogVersion event_log_version, ArtifactId aggregate, EventSourceId event_source, AggregateRootVersion aggregate_root_version, Artifact event_type) =>
            new CommittedAggregateEvent(
                new Artifact(aggregate, 0),
                aggregate_root_version,
                event_log_version,
                DateTimeOffset.UtcNow,
                event_source,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                event_type,
                false,
                events.some_event_content);

        public static CommittedEvent a_committed_event_with_type(EventLogVersion event_log_version, Artifact event_type) =>
            new CommittedEvent(
                event_log_version,
                DateTimeOffset.UtcNow,
                EventSourceId.NotSet,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                event_type,
                false,
                events.some_event_content);
    }
}