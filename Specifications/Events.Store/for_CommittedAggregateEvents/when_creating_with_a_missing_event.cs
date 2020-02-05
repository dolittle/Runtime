// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_CommittedAggregateEvents
{
    public class when_creating_with_a_missing_event : given.events_and_an_artifact
    {
        static CommittedAggregateEvents events;
        static Exception exception;

        Because of = () => exception = Catch.Exception(() =>
        {
            events = new CommittedAggregateEvents(event_source_id, aggregate_artifact.Id, aggregate_version_after, new[] { event_one, event_two });
        });

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<MissingEventsForExpectedAggregateRootVersion>();
        It should_not_be_created = () => events.ShouldBeNull();
    }
}
