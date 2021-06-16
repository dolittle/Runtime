// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_UncommittedEvents.given
{
    public abstract class events
    {
        public const bool is_public = false;
        public static EventSourceId event_source_id = Guid.NewGuid();
        public static Artifact event_a_artifact = new Artifact(Guid.Parse("d26cc060-9153-4988-8f07-3cf67f58bf47"), ArtifactGeneration.First);
        public static Artifact event_b_artifact = new Artifact(Guid.Parse("cc657c0a-2c81-4338-85a8-507f05d4fc0e"), ArtifactGeneration.First);

        public static UncommittedEvent event_one;
        public static UncommittedEvent event_two;
        public static UncommittedEvent event_three;

        Establish context = () =>
        {
            event_one = new UncommittedEvent(event_source_id, event_a_artifact, is_public, "one");
            event_two = new UncommittedEvent(event_source_id, event_a_artifact, is_public, "two");
            event_three = new UncommittedEvent(event_source_id, event_b_artifact, is_public, "three");
        };
    }
}
