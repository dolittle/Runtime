// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingStateUpdater
{
    public class from_many_events : given.all_dependencies
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
        static ProjectionReplaceResult result_after_two;
        static ProjectionReplaceResult result_after_three;

        Establish context = () =>
        {
            projection_key = "projection-key";
            event_source = Guid.Parse("642ca1f2-c8e1-4e5c-a213-246ce95a8376");
            aggregate_root_type = new Artifact(embedding.Value, ArtifactGeneration.First);
            current_state = new EmbeddingCurrentState(3, ProjectionCurrentStateType.Persisted, "state-current", projection_key);
            event_one = new CommittedAggregateEvent(aggregate_root_type, 3, 10, DateTimeOffset.Now, event_source, execution_context, event_type, false, "event-one-content");
            event_two = new CommittedAggregateEvent(aggregate_root_type, 4, 11, DateTimeOffset.Now, event_source, execution_context, event_type, true, "event-two-content");
            event_three = new CommittedAggregateEvent(aggregate_root_type, 5, 12, DateTimeOffset.Now, event_source, execution_context, event_type, false, "event-three-content");
            unprocessed_events = new CommittedAggregateEvents(event_source, aggregate_root_type.Id, new[] { event_one, event_two, event_three });
            result_after_one = new ProjectionReplaceResult("state-after-one");
            result_after_two = new ProjectionReplaceResult("state-after-two");
            result_after_three = new ProjectionReplaceResult("state-after-three");

            embedding_store.Setup(_ => _.TryGetKeys(embedding, cancellation_token)).Returns(Task.FromResult<Try<IEnumerable<ProjectionKey>>>(new[] { projection_key }));
            key_converter.Setup(_ => _.GetEventSourceIdFor(projection_key)).Returns(event_source);
            embedding_store.Setup(_ => _.TryGet(embedding, projection_key, cancellation_token)).Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state));
            embedding_store.Setup(_ => _.TryReplace(embedding, projection_key, event_three.AggregateRootVersion + 1, result_after_three.State, cancellation_token)).Returns(Task.FromResult<Try>(true));
            event_store.Setup(_ => _.FetchForAggregateAfter(event_source, embedding.Value, current_state.Version, cancellation_token)).Returns(Task.FromResult(unprocessed_events));
            projection.Setup(_ => _.Project(current_state, event_one, events_partition, cancellation_token)).Returns(Task.FromResult<IProjectionResult>(result_after_one));
            projection.Setup(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_one.State.Value), event_two, events_partition, cancellation_token)).Returns(Task.FromResult<IProjectionResult>(result_after_two));
            projection.Setup(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_two.State.Value), event_three, events_partition, cancellation_token)).Returns(Task.FromResult<IProjectionResult>(result_after_three));
        };

        static Try result;
        Because of = () => result = state_updater.TryUpdateAll(cancellation_token).GetAwaiter().GetResult();

        It should_succeed = () => result.Success.ShouldBeTrue();
        It should_ask_the_embedding_store_for_keys = () => embedding_store.Verify(_ => _.TryGetKeys(embedding, cancellation_token));
        It should_get_the_last_state_from_the_embedding_store = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key, cancellation_token));
        It should_ask_the_event_store_for_new_events = () => event_store.Verify(_ => _.FetchForAggregateAfter(event_source, embedding.Value, current_state.Version, cancellation_token));
        It should_project_the_first_event = () => projection.Verify(_ => _.Project(current_state, event_one, events_partition, cancellation_token));
        It should_project_the_second_event = () => projection.Verify(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_one.State.Value), event_two, events_partition, cancellation_token));
        It should_project_the_third_event = () => projection.Verify(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_two.State.Value), event_three, events_partition, cancellation_token));
        It should_store_the_final_state = () => embedding_store.Verify(_ => _.TryReplace(embedding, projection_key, event_three.AggregateRootVersion + 1, result_after_three.State, cancellation_token));
        It should_not_store_any_intermediate_states = () => embedding_store.VerifyNoOtherCalls();
    }
}
