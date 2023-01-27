// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_CommittedEventExtensions;

public class when_getting_stream_event_metadata
{
    static CommittedEvent committed_event;
    static StreamEventMetadata result;

    Establish context = () =>
    {
        committed_event = committed_events.a_committed_event(random.event_log_sequence_number);
    };

    Because of = () => result = committed_event.GetStreamEventMetadata();

    It should_have_the_correct_event_log_sequence_number = () => result.EventLogSequenceNumber.Should().Be(committed_event.EventLogSequenceNumber.Value);
    It should_have_the_event_source = () => result.EventSource.Should().Be(committed_event.EventSource.Value);
    It should_have_the_correct_occurred_value = () => result.Occurred.Should().Be(committed_event.Occurred.DateTime);
    It should_have_the_correct_public_value = () => result.Public.Should().Be(committed_event.Public);
    It should_have_the_correct_type_id = () => result.TypeId.Should().Be(committed_event.Type.Id.Value);
    It should_have_the_correct_type_generation = () => result.TypeGeneration.Should().Be(committed_event.Type.Generation.Value);
}