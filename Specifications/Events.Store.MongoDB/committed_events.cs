// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.MongoDB;

public static class committed_events
{
    public static CommittedAggregateEvent a_committed_aggregate_event(EventLogSequenceNumber event_log_sequence_number, ArtifactId aggregate, EventSourceId event_source, AggregateRootVersion aggregate_root_version) =>
        new(
            new Artifact(aggregate, 1266306380),
            aggregate_root_version,
            event_log_sequence_number,
            new DateTimeOffset(2732723935, TimeSpan.Zero),
            event_source,
            execution_contexts.create(),
            new Artifact(Guid.Parse("2120418a-7869-46b6-9435-09ba6ab9a4cf"), 2010766075),
            false,
            events.some_event_content);

    public static CommittedEvent a_committed_event(EventLogSequenceNumber event_log_sequence_number) =>
        new(
            event_log_sequence_number,
            new DateTimeOffset(2232571935, TimeSpan.Zero),
            "event source",
            execution_contexts.create(),
            new Artifact(Guid.Parse("e61f3f6e-fc31-4e76-9274-c37cacbb74eb"), 2405803362),
            false,
            events.some_event_content);

    public static CommittedExternalEvent a_committed_external_event(EventLogSequenceNumber event_log_sequence_number, EventLogSequenceNumber external_event_log_sequence_number) =>
        new(
            event_log_sequence_number,
            new DateTimeOffset(1242521935, TimeSpan.Zero),
            "event source 4f93-8e18-a2aadf070175",
            execution_contexts.create(),
            new Artifact(Guid.Parse("9e2f39c6-4824-4054-b714-8ccf63921cd9"), 2575047027),
            false,
            events.some_event_content,
            external_event_log_sequence_number,
            DateTimeOffset.UtcNow,
            Guid.Parse("b9113871-09de-413e-8530-ba6f1b2465cb"));
}