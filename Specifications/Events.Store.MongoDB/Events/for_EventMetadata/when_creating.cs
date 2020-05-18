// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventMetadata
{
    public class when_creating
    {
        static ulong event_log_sequence_number;
        static DateTime occurred;
        static Guid event_source;
        static Guid type_id;
        static uint type_generation;
        static bool is_public;
        static bool from_event_horizon;
        static ulong origin_event_log_sequence_number;
        static EventMetadata result;

        Establish context = () =>
        {
            occurred = DateTime.Now;
            event_source = Guid.NewGuid();
            type_id = Guid.NewGuid();
            type_generation = 3;
            is_public = true;
            from_event_horizon = false;
            origin_event_log_sequence_number = 2;
        };

        Because of = () => result = new EventMetadata(
            occurred,
            event_source,
            type_id,
            type_generation,
            is_public,
            from_event_horizon,
            origin_event_log_sequence_number);

        It should_have_the_correct_occurred_value = () => result.Occurred.ShouldEqual(occurred);
        It should_have_the_correct_event_source = () => result.EventSource.ShouldEqual(event_source);
        It should_have_the_correct_type_id = () => result.TypeId.ShouldEqual(type_id);
        It should_have_the_correct_type_generation = () => result.TypeGeneration.ShouldEqual(type_generation);
        It should_have_the_correct_public_value = () => result.Public.ShouldEqual(is_public);
        It should_have_the_correct_from_event_horizon_value = () => result.FromEventHorizon.ShouldEqual(from_event_horizon);
        It should_have_the_correct_from_origin_event_log_sequence_number = () => result.OriginEventLogSequenceNumber.ShouldEqual(origin_event_log_sequence_number);
    }
}