// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_CommittedAggregateEvents;

public class when_creating_with_a_wrong_event_source : given.events_and_an_artifact
{
    static EventSourceId wrong_event_source_id = "wrong event source id";
    static CommittedAggregateEvent wrong_event_source_event;
    static CommittedAggregateEvents events;
    static Exception exception;

    Establish context = () => wrong_event_source_event = new CommittedAggregateEvent(aggregate_artifact, aggregate_version_after, 3, DateTimeOffset.UtcNow, wrong_event_source_id, execution_contexts.create(), event_b_artifact, is_public, "wrong");

    Because of = () => exception = Catch.Exception(() => events = new CommittedAggregateEvents(event_source_id, aggregate_artifact.Id, 2, new[] { event_one, event_two, event_three, wrong_event_source_event }));

    It should_throw_an_exception = () => exception.Should().BeOfType<EventWasAppliedToOtherEventSource>();
    It should_not_be_created = () => events.Should().BeNull();
}