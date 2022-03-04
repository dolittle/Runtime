// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_ProjectManyEvents.when_projecting.committed_events;

public class and_everything_works : given.all_dependencies
{
    static ProjectionKey projection_key;
    static EventSourceId event_source;
    static Artifact aggregate_root_type;
    static EmbeddingCurrentState current_state;
    static CommittedAggregateEvent event_one;
    static CommittedAggregateEvent event_two;
    static CommittedAggregateEvent event_three;
    static CommittedAggregateEvents unprocessed_events;
    static ProjectionReplaceResult result_after_one;
    static ProjectionDeleteResult result_after_two;
    static ProjectionReplaceResult result_after_three;

    Establish context = () =>
    {
        projection_key = "projection-key-ZSF";
        event_source = "ed882f2f-9974-4334-ad43-e662d6db3396";
        aggregate_root_type = new Artifact(identifier.Value, ArtifactGeneration.First);
        current_state = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.Persisted, "state-current", projection_key);
        event_one = new CommittedAggregateEvent(aggregate_root_type, 3, 10, DateTimeOffset.Now, event_source, execution_context, event_type, false, "event-one-content");
        event_two = new CommittedAggregateEvent(aggregate_root_type, 4, 11, DateTimeOffset.Now, event_source, execution_context, event_type, true, "event-two-content");
        event_three = new CommittedAggregateEvent(aggregate_root_type, 5, 12, DateTimeOffset.Now, event_source, execution_context, event_type, false, "event-three-content");
        unprocessed_events = new CommittedAggregateEvents(event_source, aggregate_root_type.Id, new[] { event_one, event_two, event_three });
        result_after_one = new ProjectionReplaceResult("state-after-one");
        result_after_two = new ProjectionDeleteResult();
        result_after_three = new ProjectionReplaceResult("state-after-three");
        embedding
            .Setup(_ => _.Project(current_state, Moq.It.Is<UncommittedEvent>(_ => _.Content == event_one.Content), execution_context, cancellation_token))
            .Returns(Task.FromResult<IProjectionResult>(result_after_one));
        embedding
            .Setup(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_one.State.Value), Moq.It.Is<UncommittedEvent>(_ => _.Content == event_two.Content), execution_context, cancellation_token))
            .Returns(Task.FromResult<IProjectionResult>(result_after_two));
        embedding
            .Setup(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == initial_state), Moq.It.Is<UncommittedEvent>(_ => _.Content == event_three.Content), execution_context, cancellation_token))
            .Returns(Task.FromResult<IProjectionResult>(result_after_three));
    };

    static Partial<EmbeddingCurrentState> result;
    Because of = () => result = project_many_events.TryProject(current_state, unprocessed_events, execution_context, cancellation_token).GetAwaiter().GetResult();

    It should_succeed = () => result.Success.ShouldBeTrue();

    It should_return_the_third_state = () => result.Result.State.ShouldEqual(result_after_three.State);
    It should_return_a_persisted_state = () => result.Result.Type.ShouldEqual(EmbeddingCurrentStateType.Persisted);
    It should_return_the_same_key = () => result.Result.Key.ShouldEqual(projection_key);
    It should_return_the_correct_aggregate_root_version = () => result.Result.Version.Value.ShouldEqual(event_three.AggregateRootVersion.Value + 1);
    It should_project_the_first_event = () => embedding.Verify(_ => _.Project(current_state, Moq.It.Is<UncommittedEvent>(_ => _.Content == event_one.Content), execution_context, cancellation_token));
    It should_project_the_second_event = () => embedding.Verify(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_one.State.Value), Moq.It.Is<UncommittedEvent>(_ => _.Content == event_two.Content), execution_context, cancellation_token));
    It should_project_the_third_event = () => embedding.Verify(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == initial_state && _.Type == ProjectionCurrentStateType.CreatedFromInitialState), Moq.It.Is<UncommittedEvent>(_ => _.Content == event_three.Content), execution_context, cancellation_token));
}