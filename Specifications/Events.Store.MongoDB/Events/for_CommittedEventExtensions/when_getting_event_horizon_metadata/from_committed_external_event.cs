// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_CommittedEventExtensions.when_getting_event_horizon_metadata
{
    public class from_committed_external_event
    {
        static CommittedExternalEvent committed_event;
        static EventHorizonMetadata result;

        Establish context = () =>
        {
            committed_event = committed_events.a_committed_external_event(random.event_log_sequence_number, random.event_log_sequence_number);
        };

        Because of = () => result = committed_event.GetEventHorizonMetadata();

        It should_have_the_correct_consent = () => result.Consent.ShouldEqual(committed_event.Consent.Value);
        It should_have_the_correct_external_event_log_sequence_number = () => result.ExternalEventLogSequenceNumber.ShouldEqual(committed_event.ExternalEventLogSequenceNumber.Value);
        It should_be_from_event_horizon = () => result.FromEventHorizon.ShouldBeTrue();
        It should_have_the_correct_received_value = () => result.Received.ShouldEqual(committed_event.Received);
    }
}