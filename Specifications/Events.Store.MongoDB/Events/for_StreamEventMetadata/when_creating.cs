// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_StreamEventMetadata;

public class when_creating
{
    static ulong event_log_sequence_number;
    static DateTime occurred;
    static string event_source;
    static Guid type_id;
    static uint type_generation;
    static bool is_public;
    static StreamEventMetadata result;

    Establish context = () =>
    {
        event_log_sequence_number = random.event_log_sequence_number;
        occurred = new DateTime(137599714, DateTimeKind.Utc);
        event_source = "  the event source";
        type_id = Guid.Parse("24352369-fd47-4950-8e62-963f2c402970");
        type_generation = 1227724615;
        is_public = true;
    };

    Because of = () => result = new StreamEventMetadata(
        event_log_sequence_number,
        occurred,
        event_source,
        type_id,
        type_generation,
        is_public);

    It should_have_the_correct_event_log_sequence_number = () => result.EventLogSequenceNumber.ShouldEqual(event_log_sequence_number);
    It should_have_the_correct_occurred_value = () => result.Occurred.ShouldEqual(occurred);
    It should_have_the_correct_event_source = () => result.EventSource.ShouldEqual(event_source);
    It should_have_the_correct_type_id = () => result.TypeId.ShouldEqual(type_id);
    It should_have_the_correct_type_generation = () => result.TypeGeneration.ShouldEqual(type_generation);
    It should_have_the_correct_public_value = () => result.Public.ShouldEqual(is_public);
}