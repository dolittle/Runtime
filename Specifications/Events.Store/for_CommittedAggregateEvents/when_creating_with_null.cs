// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedAggregateEvents;

public class when_creating_with_null : given.events_and_an_artifact
{
    static CommittedAggregateEvents events;
    static Exception exception;

    Because of = () => exception = Catch.Exception(() => events = new CommittedAggregateEvents(event_source_id, aggregate_artifact.Id, 4, new[] { event_one, null, event_three }));

    It should_throw_an_exception = () => exception.Should().BeOfType<EventCanNotBeNull>();
    It should_not_be_created = () => events.Should().BeNull();
}