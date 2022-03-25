// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Producer.for_EventExtensions.when_converting_to_committed_event_horizon_event;

public class and_there_are_claims
{
    static CommittedEvent committed_event;
    static Events.Contracts.CommittedEvent result;

    Establish context = () => committed_event = new CommittedEvent(
        0,
        DateTimeOffset.UtcNow,
        "an event source id",
        execution_contexts.create_with_claims(new Claims(new[] { new Claim("name", "value", "valueType") })),
        artifacts.create(),
        false,
        "content");

    Because of = () => result = committed_event.ToCommittedEventHorizonEvent();

    It should_have_the_correct_event_log_sequence_number = () => result.EventLogSequenceNumber.ShouldEqual(committed_event.EventLogSequenceNumber.Value);
    It should_have_the_correct_content = () => result.Content.ShouldEqual(committed_event.Content);
    It should_have_the_correct_event_source = () => result.EventSourceId.ShouldEqual(committed_event.EventSource.Value);
    It should_not_be_an_external_event = () => result.External.ShouldBeFalse();
    It should_have_the_default_external_event_log_sequence_number = () => result.ExternalEventLogSequenceNumber.ShouldEqual(default);
    It should_not_have_external_event_received = () => result.ExternalEventReceived.ShouldBeNull();
    It should_have_the_correct_occurred_date_time = () => result.Occurred.ToDateTimeOffset().ShouldEqual(committed_event.Occurred);
    It should_not_be_public = () => result.Public.ShouldBeFalse();
    It should_have_the_correct_type_generation = () => result.EventType.Generation.ShouldEqual(committed_event.Type.Generation.Value);
    It should_have_the_correct_type_id = () => result.EventType.Id.ToGuid().ShouldEqual(committed_event.Type.Id.Value);
    It should_not_have_any_claims = () => result.ExecutionContext.ToExecutionContext().Claims.ShouldEqual(Claims.Empty);
}