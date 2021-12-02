// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventConverter.when_converting_to_event_log_event;

public class an_external_event : given.an_event_content_converter
{
    static CommittedExternalEvent committed_event;
    static IEventConverter event_converter;
    static MongoDB.Events.Event result;

    Establish context = () =>
    {
        committed_event = committed_events.a_committed_external_event(random.event_log_sequence_number, random.event_log_sequence_number);
        event_converter = new EventConverter(event_content_converter.Object);
    };

    Because of = () => result = event_converter.ToEventLogEvent(committed_event);

    It should_represent_the_same_event = () => result.ShouldBeTheSameAs(committed_event);
    It should_not_be_applied_by_aggregate = () => result.Aggregate.WasAppliedByAggregate.ShouldBeFalse();
    It should_come_from_event_horizon = () => result.EventHorizon.FromEventHorizon.ShouldBeTrue();
    It should_have_the_same_consent = () => result.EventHorizon.Consent.ShouldEqual(committed_event.Consent.Value);
    It should_have_the_same_external_event_log_sequence_number = () => result.EventHorizon.ExternalEventLogSequenceNumber.ShouldEqual(committed_event.ExternalEventLogSequenceNumber.Value);
    It shoul_have_the_same_received_value = () => result.EventHorizon.Received.ShouldEqual(committed_event.Received.UtcDateTime);
    It should_have_the_content_returned_by_the_content_converter = () => result.Content.ShouldBeTheSameAs(bson_returned_by_event_converter);
    It should_call_the_content_converter_with_the_content = () => event_content_converter.VerifyOnlyCall(_ => _.ToBson(committed_event.Content));
}