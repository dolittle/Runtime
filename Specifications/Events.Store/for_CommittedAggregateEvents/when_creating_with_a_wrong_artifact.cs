// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_CommittedAggregateEvents
{
    public class when_creating_with_a_wrong_artifact : given.events_and_an_artifact
    {
        static Artifact wrong_aggregate_artifact = new Artifact(Guid.Parse("29ca4005-a03b-4461-a93b-10260ca374ae"), ArtifactGeneration.First);
        static CommittedAggregateEvent wrong_aggregate_event;
        static CommittedAggregateEvents events;
        static Exception exception;

        Establish context = () => wrong_aggregate_event = new CommittedAggregateEvent(wrong_aggregate_artifact, aggregate_version_after + 1, 3, DateTimeOffset.UtcNow, event_source_id, execution_contexts.create(), event_b_artifact, is_public, "wrong");

        Because of = () => exception = Catch.Exception(() => events = new CommittedAggregateEvents(event_source_id, aggregate_artifact.Id, new[] { event_one, event_two, event_three, wrong_aggregate_event }));

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventWasAppliedByOtherAggregateRoot>();
        It should_not_be_created = () => events.ShouldBeNull();
    }
}
