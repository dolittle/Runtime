// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedEvents;

public class when_creating_with_out_of_order_event_log_sequence_number : given.events
{
    static CommittedEvent out_of_order_event;
    static CommittedEvents events;
    static Exception exception;

    Establish context = () => out_of_order_event = new CommittedEvent(4, DateTimeOffset.UtcNow, "some-event-source-id", execution_context, event_b_artifact, is_public, "wrong");

    Because of = () => exception = Catch.Exception(() => events = new CommittedEvents(new[] { out_of_order_event, event_one, event_two, event_three }));

    It should_throw_an_exception = () => exception.Should().BeOfType<EventLogSequenceIsOutOfOrder>();
    It should_not_be_created = () => events.Should().BeNull();
}