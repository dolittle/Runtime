// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Machine.Specifications;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Dolittle.Runtime.Events.Store.for_CommittedAggregateEventsExtension.given;

public class all_dependencies
{
    protected static ArtifactId aggregate_root_id;
    protected static EventSourceId event_source_id;
    protected static AggregateRootVersion number_of_events_in_aggregate_when_commit_happened;
    protected static ExecutionContext execution_context;
    
    protected static Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents protobuf_committed_events;
    protected static CommittedAggregateEvents result; 
    Establish context = () =>
    {
        aggregate_root_id = "a82d47c8-b444-4467-be45-7801a46f495f";
        event_source_id = "some event source";
        number_of_events_in_aggregate_when_commit_happened = 0;
        execution_context = new ExecutionContext(
            "d13f0721-50a4-4fd1-95ac-7f1721b8df9d",
            "5bdb2ecd-287e-4943-b00a-ba888993b84a",
            Version.NotSet,
            Environment.Development, 
            "4502e066-13e1-4cbf-b92e-d5de6faf58f5",
            ActivitySpanId.CreateRandom(),
            Claims.Empty,
            CultureInfo.InvariantCulture);

    };

    protected static Contracts.CommittedAggregateEvents with_committed_events(params Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent[] events)
    {
        var aggregate_version_after_commit = number_of_events_in_aggregate_when_commit_happened + (ulong)events.Length;
        var result = new Contracts.CommittedAggregateEvents
        {
            AggregateRootId = aggregate_root_id.ToProtobuf(),
            EventSourceId = event_source_id,
            // The aggregate root version of the committed aggregate events is the version of the last event, meaning the aggregate root version before commit happened + number of events - 1
            AggregateRootVersion = aggregate_version_after_commit - 1,
            CurrentAggregateRootVersion =  aggregate_version_after_commit
        };
        result.Events.AddRange(events);
        return result;
    }

    protected static Contracts.CommittedAggregateEvents.Types.CommittedAggregateEvent with_committed_event(
        string content,
        EventLogSequenceNumber event_log_sequence_number,
        ArtifactId event_type_id,
        ArtifactGeneration event_type_generation,
        bool isPublic)
        => new()
        {
            Content = content,
            ExecutionContext = execution_context.ToProtobuf(),
            Occurred = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
            Public = isPublic,
            EventLogSequenceNumber = event_log_sequence_number,
            EventType = new Artifact(event_type_id, event_type_generation).ToProtobuf()
        };
    
    protected static void should_have_correct_event(int index, AggregateRootVersion expected_version)
    {
        var committed_event = result[index];
        var protobuf_committed_event = protobuf_committed_events.Events[index];

        committed_event.AggregateRoot.ShouldEqual(new Artifact(protobuf_committed_events.AggregateRootId.ToGuid(), ArtifactGeneration.First));
        committed_event.AggregateRootVersion.ShouldEqual(expected_version);
        committed_event.Content.ShouldEqual(protobuf_committed_event.Content);
        committed_event.Occurred.ShouldEqual(protobuf_committed_event.Occurred.ToDateTimeOffset());
        committed_event.Public.ShouldEqual(protobuf_committed_event.Public);
        committed_event.Type.ShouldEqual(protobuf_committed_event.EventType.ToArtifact());
        committed_event.EventSource.Value.ShouldEqual(protobuf_committed_events.EventSourceId);
        committed_event.ExecutionContext.ShouldEqual(protobuf_committed_event.ExecutionContext.ToExecutionContext());
        committed_event.EventLogSequenceNumber.Value.ShouldEqual(protobuf_committed_event.EventLogSequenceNumber);
    }
}