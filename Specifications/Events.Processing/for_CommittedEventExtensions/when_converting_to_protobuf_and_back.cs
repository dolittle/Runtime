// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_CommittedEventExtensions;

public class when_converting_to_protobuf_and_back
{
    static CommittedEvent committed_event;
    static CommittedEvent result;

    Establish context = () => committed_event = committed_events.single();

    Because of = () => result = committed_event.ToProtobuf().ToCommittedEvent();

    It should_hold_same_event_log_sequence_number = () => result.EventLogSequenceNumber.ShouldEqual(committed_event.EventLogSequenceNumber);
    It should_hold_same_occurred = () => result.Occurred.ShouldEqual(committed_event.Occurred);
    It should_hold_same_event_source_id = () => result.EventSource.ShouldEqual(committed_event.EventSource);
    It should_hold_same_correlation_id = () => result.ExecutionContext.CorrelationId.ShouldEqual(committed_event.ExecutionContext.CorrelationId);
    It should_hold_same_microservice = () => result.ExecutionContext.Microservice.ShouldEqual(committed_event.ExecutionContext.Microservice);
    It should_hold_same_tenant = () => result.ExecutionContext.Tenant.ShouldEqual(committed_event.ExecutionContext.Tenant);
    It should_hold_same_artifact_id_for_type = () => result.Type.Id.ShouldEqual(committed_event.Type.Id);
    It should_hold_same_artiact_geneartion_for_type = () => result.Type.Generation.ShouldEqual(committed_event.Type.Generation);
    It should_hold_same_content = () => result.Content.ShouldEqual(committed_event.Content);
}