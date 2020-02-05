// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_CommittedEvents
{
    public class when_creating_empty
    {
        static CommittedEvents events;

        Because of = () =>
        {
            events = new CommittedEvents(Array.Empty<CommittedEvent>());
        };

        It should_not_have_events = () => events.HasEvents.ShouldBeFalse();
        It should_have_a_count_of_zero = () => events.Count.ShouldEqual(0);
    }
}
