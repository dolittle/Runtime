// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Execution;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedAggregateEvent
{
    public class when_creating_event
    {
        static ArtifactId aggregate_root_id;
        static ArtifactGeneration aggregate_root_generation;
        static AggregateRootVersion aggregate_root_version;
        static EventLogSequenceNumber event_log_sequence_number;
        static DateTimeOffset occurred;
        static EventSourceId event_source;
        static ExecutionContext execution_context;
        static ArtifactId artifact_id;
        static ArtifactGeneration artifact_generation;
        static bool is_public;
        static string content;
        static CommittedAggregateEvent @event;

        Establish context = () =>
        {
            aggregate_root_id = Guid.NewGuid();
            aggregate_root_generation = 0;
            aggregate_root_version = 1;
            event_log_sequence_number = 2;
            occurred = DateTimeOffset.Now;
            event_source = Guid.NewGuid();
            execution_context = execution_contexts.create();
            artifact_id = Guid.NewGuid();
            artifact_generation = 4;
            is_public = false;
            content = "some content";
        };

        Because of = () =>
            @event = new CommittedAggregateEvent(
                new Artifact(aggregate_root_id, aggregate_root_generation),
                aggregate_root_version,
                event_log_sequence_number,
                occurred,
                event_source,
                execution_context,
                new Artifact(artifact_id, artifact_generation),
                is_public,
                content);

        It should_have_the_correct_aggregate_root_artifact_id = () => @event.AggregateRoot.Id.ShouldEqual(aggregate_root_id);
        It should_have_the_correct_aggregate_root_artifact_generation = () => @event.AggregateRoot.Generation.ShouldEqual(aggregate_root_generation);
        It should_have_the_correct_aggregate_root_version = () => @event.AggregateRootVersion.ShouldEqual(aggregate_root_version);
        It should_have_the_correct_event_log_sequence_number = () => @event.EventLogSequenceNumber.ShouldEqual(event_log_sequence_number);
        It should_have_the_correct_occurred_date = () => @event.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_event_source = () => @event.EventSource.ShouldEqual(event_source);
        It should_have_the_correct_execution_context = () => @event.ExecutionContext.ShouldEqual(execution_context);
        It should_have_the_correct_artifact_id = () => @event.Type.Id.ShouldEqual(artifact_id);
        It should_have_the_correct_artifact_generation = () => @event.Type.Generation.ShouldEqual(artifact_generation);
        It should_have_the_correct_is_public_value = () => @event.Public.ShouldEqual(is_public);
        It should_have_the_correct_content = () => @event.Content.ShouldEqual(content);
    }
}