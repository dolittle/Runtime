// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventHorizonMetadata
{
    public class when_creating
    {
        static ulong external_event_log_sequence_number;
        static DateTime received;
        static Guid consent;
        static EventHorizonMetadata result;

        Establish context = () =>
        {
            external_event_log_sequence_number = random.event_log_sequence_number;
            received = new DateTime(2077067360, DateTimeKind.Utc);
            consent = Guid.Parse("94e80e63-f2ea-4c6a-8643-70da16d7d099");
        };

        Because of = () => result = new EventHorizonMetadata(
            external_event_log_sequence_number,
            received,
            consent);

        It should_have_the_correct_external_event_log_sequence_number = () => result.ExternalEventLogSequenceNumber.ShouldEqual(external_event_log_sequence_number);
        It should_have_the_correct_received_value = () => result.Received.ShouldEqual(received);
        It should_have_the_correct_consent = () => result.Consent.ShouldEqual(consent);
    }
}