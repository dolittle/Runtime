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

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingStateUpdater.when_updating
{
    public class and_two_of_three_keys_fails : given.all_dependencies
    {
        static Exception exception_a;
        static Exception exception_b;
        static ProjectionKey projection_key_a;
        static EventSourceId event_source_a;
        static EmbeddingCurrentState current_state_a;
        static CommittedAggregateEvent committed_event_a;
        static CommittedAggregateEvents unprocessed_events_a;
        static EmbeddingCurrentState projection_result_a;
        static ProjectionKey projection_key_b;
        static EventSourceId event_source_b;
        static ProjectionKey projection_key_c;
        static EventSourceId event_source_c;
        static EmbeddingCurrentState current_state_c;
        static CommittedAggregateEvent committed_event_c;
        static CommittedAggregateEvents unprocessed_events_c;
        static EmbeddingCurrentState projection_result_c;

        Establish context = () =>
        {
            exception_a = new Exception();
            exception_b = new Exception();

            projection_key_a = "key-a";
            event_source_a = Guid.Parse("f0cbe001-65a4-49d2-adcb-ff4d42e92223");
            current_state_a = new EmbeddingCurrentState(1, EmbeddingCurrentStateType.Persisted, "state-current-a", projection_key_a);
            committed_event_a = new CommittedAggregateEvent(new Artifact(embedding.Value, ArtifactGeneration.First), 3, 10, DateTimeOffset.Now, event_source_a, execution_context, event_type, false, "event-one-content");
            unprocessed_events_a = new CommittedAggregateEvents(event_source_a, embedding.Value, new[] { committed_event_a });
            projection_result_a = new EmbeddingCurrentState(current_state_a.Version + 1, EmbeddingCurrentStateType.Deleted, current_state_a.State, current_state_a.Key);

            projection_key_b = "key-b";
            event_source_b = Guid.Parse("760a14d6-639d-4eba-9e7e-48675631bdd2");

            projection_key_c = "key-c";
            event_source_c = Guid.Parse("313ef6a7-62da-4761-b054-fd33d89d5ebd");
            current_state_c = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.CreatedFromInitialState, "state-initial-c", projection_key_c);
            committed_event_c = new CommittedAggregateEvent(new Artifact(embedding.Value, ArtifactGeneration.First), 3, 10, DateTimeOffset.Now, event_source_c, execution_context, event_type, false, "event-one-content");
            unprocessed_events_c = new CommittedAggregateEvents(event_source_c, embedding.Value, new[] { committed_event_c });
            projection_result_c = new EmbeddingCurrentState(current_state_c.Version + 1, EmbeddingCurrentStateType.Persisted, current_state_c.State, current_state_c.Key);

            embedding_store
                .Setup(_ => _.TryGetKeys(embedding, cancellation_token))
                .Returns(Task.FromResult<Try<IEnumerable<ProjectionKey>>>(new[] { projection_key_a, projection_key_b, projection_key_c }));

            key_converter
                .Setup(_ => _.GetEventSourceIdFor(projection_key_a))
                .Returns(event_source_a);
            embedding_store
                .Setup(_ => _.TryGet(embedding, projection_key_a, cancellation_token))
                .Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state_a));
            event_store
                .Setup(_ => _.FetchForAggregateAfter(event_source_a, embedding.Value, current_state_a.Version, cancellation_token))
                .Returns(Task.FromResult(unprocessed_events_a));
            project_many_events
                .Setup(_ => _.TryProject(current_state_a, unprocessed_events_a, cancellation_token))
                .Returns(Task.FromResult<Partial<EmbeddingCurrentState>>(projection_result_a));
            embedding_store
                .Setup(_ => _.TryRemove(embedding, projection_key_a, projection_result_a.Version, cancellation_token))
                .Returns(Task.FromResult<Try>(exception_a));

            key_converter
                .Setup(_ => _.GetEventSourceIdFor(projection_key_b))
                .Returns(event_source_b);
            embedding_store
                .Setup(_ => _.TryGet(embedding, projection_key_b, cancellation_token))
                .Returns(Task.FromResult<Try<EmbeddingCurrentState>>(exception_b));

            key_converter
                .Setup(_ => _.GetEventSourceIdFor(projection_key_c))
                .Returns(event_source_c);
            embedding_store
                .Setup(_ => _.TryGet(embedding, projection_key_c, cancellation_token))
                .Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state_c));
            event_store
                .Setup(_ => _.FetchForAggregateAfter(event_source_c, embedding.Value, current_state_c.Version, cancellation_token))
                .Returns(Task.FromResult(unprocessed_events_c));
            project_many_events
                .Setup(_ => _.TryProject(current_state_c, unprocessed_events_c, cancellation_token))
                .Returns(Task.FromResult<Partial<EmbeddingCurrentState>>(projection_result_c));
            embedding_store
                .Setup(_ => _.TryReplace(embedding, projection_key_c, projection_result_c.Version, projection_result_c.State, cancellation_token))
                .Returns(Task.FromResult<Try>(true));
        };

        static Try result;
        Because of = () => result = state_updater.TryUpdateAll(cancellation_token).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_with_the_first_error = () => result.Exception.ShouldBeTheSameAs(exception_a);
        It should_ask_the_embedding_store_for_keys = () => embedding_store.Verify(_ => _.TryGetKeys(embedding, cancellation_token));
        It should_get_the_last_state_from_the_embedding_store_for_a = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_a, cancellation_token));
        It should_ask_the_event_store_for_new_events_for_a = () => event_store.Verify(_ => _.FetchForAggregateAfter(event_source_a, embedding.Value, current_state_a.Version, cancellation_token));
        It should_project_the_events_for_a = () => project_many_events.Verify(_ => _.TryProject(current_state_a, unprocessed_events_a, cancellation_token));
        It should_delete_for_a = () => embedding_store.Verify(_ => _.TryRemove(embedding, projection_key_a, projection_result_a.Version, cancellation_token));
        It should_get_the_last_state_from_the_embedding_store_for_b = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_b, cancellation_token));
        It should_get_the_last_state_from_the_embedding_store_for_c = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_c, cancellation_token));
        It should_ask_the_event_store_for_new_events_for_c = () => event_store.Verify(_ => _.FetchForAggregateAfter(event_source_c, embedding.Value, current_state_c.Version, cancellation_token));
        It should_project_the_events_for_c = () => project_many_events.Verify(_ => _.TryProject(current_state_c, unprocessed_events_c, cancellation_token));
        It should_update_for_c = () => embedding_store.Verify(_ => _.TryReplace(embedding, projection_key_c, projection_result_c.Version, projection_result_c.State, cancellation_token));
    }
}
