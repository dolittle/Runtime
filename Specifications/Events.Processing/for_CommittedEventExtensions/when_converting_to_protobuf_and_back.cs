// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_CommittedEventExtensions
{
    public class when_converting_to_protobuf_and_back
    {
        static Store.CommittedEvent committed_event;
        static Store.CommittedEvent result;

        Establish context = () => committed_event = committed_events.single();

        Because of = () => result = committed_event.ToProtobuf().ToCommittedEvent();

        It should_hold_same_event_log_version = () => result.EventLogVersion.ShouldEqual(committed_event.EventLogVersion);
        It should_hold_same_occurred = () => result.Occurred.ShouldEqual(committed_event.Occurred);
        It should_hold_same_event_source_id = () => result.EventSource.ShouldEqual(committed_event.EventSource);
        It should_hold_same_correlation_id = () => result.CorrelationId.ShouldEqual(committed_event.CorrelationId);
        It should_hold_same_microservice = () => result.Microservice.ShouldEqual(committed_event.Microservice);
        It should_hold_same_tenant = () => result.Tenant.ShouldEqual(committed_event.Tenant);
        It should_hold_same_cause_type = () => result.Cause.Type.ShouldEqual(committed_event.Cause.Type);
        It should_hold_same_cause_position = () => result.Cause.Position.ShouldEqual(committed_event.Cause.Position);
        It should_hold_same_artifact_id_for_type = () => result.Type.Id.ShouldEqual(committed_event.Type.Id);
        It should_hold_same_artiact_geneartion_for_type = () => result.Type.Generation.ShouldEqual(committed_event.Type.Generation);
        It should_hold_same_content = () => result.Content.ShouldEqual(committed_event.Content);
    }
}