// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_CommittedEvents
{
    public class when_creating_a_new_committed_events_with_one_event : given.two_committed_events
    {
        static CommittedEvents events;

        Because of = () => events = new CommittedEvents(new CommittedEvent[] { first_event });

        It should_have_events = () => events.HasEvents.ShouldBeTrue();
        It should_have_a_count_of_one = () => events.Count.ShouldEqual(1);
    }
}
