// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Execution;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedEvent;

public class when_creating_event
{
    static EventLogSequenceNumber event_log_sequence_number;
    static DateTimeOffset occurred;
    static EventSourceId event_source;
    static ExecutionContext execution_context;
    static ArtifactId artifact_id;
    static ArtifactGeneration artifact_generation;
    static bool is_public;
    static string content;
    static CommittedEvent @event;

    Establish context = () =>
    {
        event_log_sequence_number = 0;
        occurred = DateTimeOffset.Now;
        event_source = "a random event source";
        execution_context = execution_contexts.create();
        artifact_id = Guid.NewGuid();
        artifact_generation = 2;
        is_public = false;
        content = "some content";
    };

    Because of = () =>
        @event = new CommittedEvent(
            event_log_sequence_number,
            occurred,
            event_source,
            execution_context,
            new Artifact(artifact_id, artifact_generation),
            is_public,
            content);

    It should_have_the_correct_event_log_sequence_number = () => @event.EventLogSequenceNumber.Should().Be(event_log_sequence_number);
    It should_have_the_correct_occurred_date = () => @event.Occurred.Should().Be(occurred);
    It should_have_the_correct_event_source = () => @event.EventSource.Should().Be(event_source);
    It should_have_the_correct_execution_context = () => @event.ExecutionContext.Should().Be(execution_context);
    It should_have_the_correct_artifact_id = () => @event.Type.Id.Should().Be(artifact_id);
    It should_have_the_correct_artifact_generation = () => @event.Type.Generation.Should().Be(artifact_generation);
    It should_have_the_correct_is_public_value = () => @event.Public.Should().Be(is_public);
    It should_have_the_correct_content = () => @event.Content.Should().Be(content);
}