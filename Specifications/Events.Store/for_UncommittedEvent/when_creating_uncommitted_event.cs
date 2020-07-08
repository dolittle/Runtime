// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.for_UncommittedEvent
{
    public class when_creating_uncommitted_event
    {
        static EventSourceId event_source_id;
        static ArtifactId artifact_id;
        static ArtifactGeneration artifact_generation;
        static bool is_public;
        static string content;
        static UncommittedEvent uncommitted_event;

        Establish context = () =>
        {
            event_source_id = Guid.NewGuid();
            artifact_id = Guid.NewGuid();
            artifact_generation = 0;
            is_public = false;
            content = "content";
        };

        Because of = () => uncommitted_event = new UncommittedEvent(event_source_id, new Artifact(artifact_id, artifact_generation), is_public, content);

        It should_have_the_correct_artifact_id = () => uncommitted_event.Type.Id.ShouldEqual(artifact_id);
        It should_have_the_correct_artifact_generation = () => uncommitted_event.Type.Generation.ShouldEqual(artifact_generation);
        It should_have_the_correct_is_public_value = () => uncommitted_event.Public.ShouldEqual(is_public);
        It should_have_the_correct_content = () => uncommitted_event.Content.ShouldEqual(content);
    }
}