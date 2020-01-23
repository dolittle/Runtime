// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_CommittedEvents
{
    public class when_creating_a_new_committed_events_with_two_events_out_of_order : given.two_committed_events
    {
        static CommittedEvents events;
        static Exception exception;

        Because of = () => exception = Catch.Exception(() =>
        {
            events = new CommittedEvents(new CommittedEvent[] { second_event, first_event });
        });

        It should_not_be_created = () => events.ShouldBeNull();
        It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventLogVersionIsOutOfOrder>();
    }
}
