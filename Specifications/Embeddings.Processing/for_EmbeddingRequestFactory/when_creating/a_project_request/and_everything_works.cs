// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingRequestFactory.when_creating.a_project_request;

public class and_everything_works : given.all_dependencies
{
    static UncommittedEvent @event;
    Establish context = () =>
    {
        @event = new UncommittedEvent(
            "2b70d917-46e8-485f-8a3e-f12bd6392e8f",
            new Artifacts.Artifact(
                "97dae10c-6986-46c4-be68-2b763ae565bf",
                Artifacts.ArtifactGeneration.First
            ),
            false,
            "content");
    };

    static Contracts.EmbeddingRequest result;

    Because of = () => result = factory.Create(current_state, @event);

    It should_be_a_projection_request = () => result.RequestCase.ShouldEqual(Contracts.EmbeddingRequest.RequestOneofCase.Projection);
    It should_have_the_correct_type = () => result.Projection.CurrentState.Type.ShouldEqual(current_state.Type.ToProtobuf());
    It should_have_the_correct_state = () => result.Projection.CurrentState.State.ShouldEqual(current_state.State.Value);
    It should_have_the_correct_key = () => result.Projection.CurrentState.Key.ShouldEqual(current_state.Key.Value);
    It should_have_the_correct_event_source_id = () => result.Projection.Event.EventSourceId.ShouldEqual(@event.EventSource.Value);
    It should_have_the_correct_event_artifact_id = () => result.Projection.Event.EventType.Id.ShouldEqual(@event.Type.Id.Value.ToProtobuf());
    It should_have_the_correct_event_artifact_generation = () => result.Projection.Event.EventType.Generation.ShouldEqual(@event.Type.Generation.Value);
    It should_have_the_correct_event_content = () => result.Projection.Event.Content.ShouldEqual(@event.Content);
    It should_have_the_correct_event_public_value = () => result.Projection.Event.Public.ShouldEqual(@event.Public);

}