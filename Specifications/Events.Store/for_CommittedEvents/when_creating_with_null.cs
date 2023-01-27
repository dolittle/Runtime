// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedEvents;

public class when_creating_with_null : given.events
{
    static CommittedEvents events;
    static Exception exception;

    Because of = () => exception = Catch.Exception(() => events = new CommittedEvents(new[] { event_one, null, event_three }));

    It should_throw_an_exception = () => exception.Should().BeOfType<EventCanNotBeNull>();
    It should_not_be_created = () => events.Should().BeNull();
}