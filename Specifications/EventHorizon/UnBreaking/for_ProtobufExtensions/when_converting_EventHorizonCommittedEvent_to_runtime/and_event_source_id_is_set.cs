// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Store;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.UnBreaking.for_ProtobufExtensions.when_converting_EventHorizonCommittedEvent_to_runtime;

public class and_event_source_id_is_set : given.a_committed_event
{
    static EventSourceId event_source; 
    static CommittedEvent result;

    Establish context = () =>
    {
        event_source = "some event source";
        committed_event.EventSourceId = event_source;
    };

    Because of = () => result = committed_event.ToCommittedEvent();

    It should_have_the_correct_event_source = () => result.EventSource.Should().Be(event_source);
}