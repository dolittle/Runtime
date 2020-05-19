// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventConverter.when_converting_to_event_log_event
{
    public class an_external_event
    {
        static CommittedExternalEvent committed_event;
        static IEventConverter event_converter;
        static MongoDB.Events.Event result;

        Establish context = () =>
        {
            committed_event = committed_events.a_committed_external_event(random.event_log_sequence_number, random.event_log_sequence_number);
            event_converter = new EventConverter();
        };

        Because of = () => result = event_converter.ToEventLogEvent(committed_event);

        It should_have_the_same_content = () => result.Content.ToString().ShouldEqual(committed_event.Content);
        It should_represent_the_same_event = () => result.ShouldBeTheSameAs(committed_event);
        It should_not_be_applied_by_aggregate = () => result.Aggregate.WasAppliedByAggregate.ShouldBeFalse();
        It should_not_come_from_event_horizon = () => result.EventHorizonMetadata.FromEventHorizon.ShouldBeTrue();
        It should_have_the_same_consent = () => result.EventHorizonMetadata.Consent.ShouldEqual(committed_event.Consent.Value);
        It should_have_the_same_external_event_log_sequence_number = () => result.EventHorizonMetadata.ExternalEventLogSequenceNumber.ShouldEqual(committed_event.ExternalEventLogSequenceNumber.Value);
        It shoul_have_the_same_received_value = () => result.EventHorizonMetadata.Received.ShouldEqual(committed_event.Received.UtcDateTime);
    }
}