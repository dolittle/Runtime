// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_UncommittedAggregateEvents;

public class when_creating_with_null : given.events_and_an_aggregate
{
    static UncommittedAggregateEvents events;
    static Exception exception;

    Because of = () => exception = Catch.Exception(() =>
    {
        events = new UncommittedAggregateEvents(event_source_id, aggregate_artifact, aggregate_version, new[] { event_one, null, event_three });
    });

    It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventCanNotBeNull>();
    It should_not_be_created = () => events.ShouldBeNull();
}