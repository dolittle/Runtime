// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Execution;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_CommittedEvents.given;

public abstract class events
{
    public const bool is_public = false;
    public static ExecutionContext execution_context = execution_contexts.create();

    public static Artifact event_a_artifact = new(Guid.Parse("d26cc060-9153-4988-8f07-3cf67f58bf47"), ArtifactGeneration.First);
    public static Artifact event_b_artifact = new(Guid.Parse("cc657c0a-2c81-4338-85a8-507f05d4fc0e"), ArtifactGeneration.First);

    public static CommittedEvent event_one;
    public static CommittedEvent event_two;
    public static CommittedEvent event_three;

    Establish context = () =>
    {
        event_one = new CommittedEvent(0, DateTimeOffset.UtcNow, "event-one-event-source", execution_context, event_a_artifact, is_public, "one");
        event_two = new CommittedEvent(1, DateTimeOffset.UtcNow, "event-two-event-source", execution_context, event_a_artifact, is_public, "two");
        event_three = new CommittedEvent(2, DateTimeOffset.UtcNow, "event-three-event-source", execution_context, event_b_artifact, is_public, "three");
    };
}