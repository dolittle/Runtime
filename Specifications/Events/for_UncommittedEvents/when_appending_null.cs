// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_UncommittedEvents
{
    public class when_appending_null : given.Events
    {
        static UncommittedEvents events;
        static Exception exception;

        Establish context = () => events = new UncommittedEvents();

        Because of = () => exception = Catch.Exception(() => events.Append(null));

        It should_not_have_events = () => events.HasEvents.ShouldBeFalse();
        It should_have_a_count_of_zero = () => events.Count.ShouldEqual(0);
        It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventCanNotBeNull>();
    }
}
