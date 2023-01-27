// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_UncommittedEvents;

public class when_creating_empty
{
    static UncommittedEvents events;

    Because of = () =>
    {
        events = new UncommittedEvents(Array.Empty<UncommittedEvent>());
    };

    It should_be_empty = () => events.Should().BeEmpty();
    It should_not_have_events = () => events.HasEvents.Should().BeFalse();
    It should_have_a_count_of_zero = () => events.Count.Should().Be(0);
}