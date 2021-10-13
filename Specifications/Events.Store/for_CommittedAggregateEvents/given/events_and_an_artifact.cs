// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Execution;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_CommittedAggregateEvents.given
{
    public abstract class events_and_an_artifact
    {
        public const bool is_public = false;
        public static EventSourceId event_source_id = "another-event source-id";
        public static Artifact aggregate_artifact = new Artifact(Guid.Parse("28238ebc-6454-4229-8891-5798ecb1875f"), ArtifactGeneration.First);
        public static AggregateRootVersion aggregate_version_before = 0;
        public static AggregateRootVersion aggregate_version_after = 3;

        public static ExecutionContext execution_context = execution_contexts.create();

        public static Artifact event_a_artifact = new Artifact(Guid.Parse("d26cc060-9153-4988-8f07-3cf67f58bf47"), ArtifactGeneration.First);
        public static Artifact event_b_artifact = new Artifact(Guid.Parse("cc657c0a-2c81-4338-85a8-507f05d4fc0e"), ArtifactGeneration.First);

        public static CommittedAggregateEvent event_one;
        public static CommittedAggregateEvent event_two;
        public static CommittedAggregateEvent event_three;

        Establish context = () =>
        {
            event_one = new CommittedAggregateEvent(aggregate_artifact, 0, 0, DateTimeOffset.UtcNow, event_source_id, execution_context, event_a_artifact, is_public, "one");
            event_two = new CommittedAggregateEvent(aggregate_artifact, 1, 1, DateTimeOffset.UtcNow, event_source_id, execution_context, event_a_artifact, is_public, "two");
            event_three = new CommittedAggregateEvent(aggregate_artifact, 2, 2, DateTimeOffset.UtcNow, event_source_id, execution_context, event_b_artifact, is_public, "three");
        };
    }
}
