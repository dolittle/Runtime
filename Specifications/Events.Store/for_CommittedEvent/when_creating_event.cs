// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Tenancy;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedEvent
{
    public class when_creating_event
    {
        static EventLogSequenceNumber event_log_sequence_number;
        static DateTimeOffset occurred;
        static EventSourceId event_source;
        static CorrelationId correlation_id;
        static Microservice microservice;
        static TenantId tenant;
        static CauseType cause_type;
        static CauseLogPosition cause_position;
        static ArtifactId artifact_id;
        static ArtifactGeneration artifact_generation;
        static bool is_public;
        static string content;
        static CommittedEvent @event;

        Establish context = () =>
        {
            event_log_sequence_number = 0;
            occurred = DateTimeOffset.Now;
            event_source = Guid.NewGuid();
            correlation_id = Guid.NewGuid();
            microservice = Guid.NewGuid();
            tenant = Guid.NewGuid();
            cause_type = CauseType.Command;
            cause_position = 1;
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
                correlation_id,
                microservice,
                tenant,
                new Cause(cause_type, cause_position),
                new Artifact(artifact_id, artifact_generation),
                is_public,
                content);

        It should_have_the_correct_event_log_sequence_number = () => @event.EventLogSequenceNumber.ShouldEqual(event_log_sequence_number);
        It should_have_the_correct_occurred_date = () => @event.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_event_source = () => @event.EventSource.ShouldEqual(event_source);
        It should_have_the_correct_correlation_id = () => @event.CorrelationId.ShouldEqual(correlation_id);
        It should_have_the_correct_microservice = () => @event.Microservice.ShouldEqual(microservice);
        It should_have_the_correct_tenant = () => @event.Tenant.ShouldEqual(tenant);
        It should_have_the_correct_cause_type = () => @event.Cause.Type.ShouldEqual(cause_type);
        It should_have_the_correct_cause_position = () => @event.Cause.Position.ShouldEqual(cause_position);
        It should_have_the_correct_artifact_id = () => @event.Type.Id.ShouldEqual(artifact_id);
        It should_have_the_correct_artifact_generation = () => @event.Type.Generation.ShouldEqual(artifact_generation);
        It should_have_the_correct_is_public_value = () => @event.Public.ShouldEqual(is_public);
        It should_have_the_correct_content = () => @event.Content.ShouldEqual(content);
    }
}