// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_CommittedAggregateEvents
{
    public class when_creating_with_an_out_of_order_aggregate_version : given.events_and_an_artifact
    {
        static CommittedAggregateEvent out_of_order_event;
        static CommittedAggregateEvents events;
        static Exception exception;

        Establish context = () =>
        {
            out_of_order_event = new CommittedAggregateEvent(aggregate_artifact, 10, 3, DateTimeOffset.UtcNow, event_source_id, correlation_id, microservice_id, tenant_id, cause, event_b_artifact, "wrong");
        };

        Because of = () => exception = Catch.Exception(() =>
        {
            events = new CommittedAggregateEvents(event_source_id, aggregate_artifact.Id, aggregate_version_after + 1, new[] { event_one, event_two, event_three, out_of_order_event });
        });

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<AggregateRootVersionIsOutOfOrder>();
        It should_not_be_created = () => events.ShouldBeNull();
    }
}
