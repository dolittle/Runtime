// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_CommittedEventExtensions;

public class when_getting_event_metadata
{
    static CommittedEvent committed_event;
    static EventMetadata result;

    Establish context = () =>
    {
        committed_event = committed_events.a_committed_event(random.event_log_sequence_number);
    };

    Because of = () => result = committed_event.GetEventMetadata();

    It should_have_the_event_source = () => result.EventSource.ShouldEqual(committed_event.EventSource.Value);
    It should_have_the_correct_occurred_value = () => result.Occurred.ShouldEqual(committed_event.Occurred.DateTime);
    It should_have_the_correct_public_value = () => result.Public.ShouldEqual(committed_event.Public);
    It should_have_the_correct_type_id = () => result.TypeId.ShouldEqual(committed_event.Type.Id.Value);
    It should_have_the_correct_type_generation = () => result.TypeGeneration.ShouldEqual(committed_event.Type.Generation.Value);
}