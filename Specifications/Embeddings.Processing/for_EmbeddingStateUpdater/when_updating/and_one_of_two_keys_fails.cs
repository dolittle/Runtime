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

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingStateUpdater.when_updating;

public class and_one_of_two_keys_fails : given.all_dependencies
{
    static Exception exception;
    static ProjectionKey projection_key_a;
    static EmbeddingCurrentState current_state_a;
    static CommittedAggregateEvent unprocessed_event_a;
    static CommittedAggregateEvents unprocessed_events_a;
    static EmbeddingCurrentState projection_result_a;
    static ProjectionKey projection_key_b;
    static EmbeddingCurrentState current_state_b;
    static CommittedAggregateEvent unprocessed_event_b;
    static CommittedAggregateEvents unprocessed_events_b;
    static EmbeddingCurrentState projection_result_b;

    Establish context = () =>
    {
        exception = new Exception();

        projection_key_a = "key-a";
        current_state_a = new EmbeddingCurrentState(1, EmbeddingCurrentStateType.Persisted, "state-current-a", projection_key_a);
        unprocessed_event_a = new CommittedAggregateEvent(new Artifact(embedding.Value, ArtifactGeneration.First), 1, 10, DateTimeOffset.Now, projection_key_a.Value, execution_context, event_type, false, "event-one-content");
        unprocessed_events_a = new CommittedAggregateEvents(projection_key_a.Value, embedding.Value, new[] { unprocessed_event_a });
        projection_result_a = new EmbeddingCurrentState(current_state_a.Version.Value + 1, EmbeddingCurrentStateType.Deleted, current_state_a.State, current_state_a.Key);

        projection_key_b = "key-b";
        current_state_b = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.CreatedFromInitialState, "state-initial-b", projection_key_b);
        unprocessed_event_b = new CommittedAggregateEvent(new Artifact(embedding.Value, ArtifactGeneration.First), 1, 10, DateTimeOffset.Now, projection_key_b.Value, execution_context, event_type, false, "event-one-content");
        unprocessed_events_b = new CommittedAggregateEvents(projection_key_b.Value, embedding.Value, new[] { unprocessed_event_b });
        projection_result_b = new EmbeddingCurrentState(current_state_b.Version.Value + 1, EmbeddingCurrentStateType.Persisted, current_state_b.State, current_state_b.Key);

        embedding_store
            .Setup(_ => _.TryGetKeys(embedding, cancellation_token))
            .Returns(Task.FromResult<Try<IEnumerable<ProjectionKey>>>(new[] { projection_key_a, projection_key_b }));

        embedding_store
            .Setup(_ => _.TryGet(embedding, projection_key_a, cancellation_token))
            .Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state_a));
        event_store
            .Setup(_ => _.FetchForAggregateAfter(projection_key_a.Value, embedding.Value, current_state_a.Version, cancellation_token))
            .Returns(Task.FromResult(unprocessed_events_a));
        project_many_events
            .Setup(_ => _.TryProject(current_state_a, unprocessed_events_a, cancellation_token))
            .Returns(Task.FromResult<Partial<EmbeddingCurrentState>>(projection_result_a));
        embedding_store
            .Setup(_ => _.TryRemove(embedding, projection_key_a, projection_result_a.Version, cancellation_token))
            .Returns(Task.FromResult<Try>(exception));

        embedding_store
            .Setup(_ => _.TryGet(embedding, projection_key_b, cancellation_token))
            .Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state_b));
        event_store
            .Setup(_ => _.FetchForAggregateAfter(projection_key_b.Value, embedding.Value, current_state_b.Version, cancellation_token))
            .Returns(Task.FromResult(unprocessed_events_b));
        project_many_events
            .Setup(_ => _.TryProject(current_state_b, unprocessed_events_b, cancellation_token))
            .Returns(Task.FromResult<Partial<EmbeddingCurrentState>>(projection_result_b));
        embedding_store
            .Setup(_ => _.TryReplace(embedding, projection_key_b, projection_result_b.Version, projection_result_b.State, cancellation_token))
            .Returns(Task.FromResult(Try.Succeeded()));
    };

    static Try result;
    Because of = () => result = state_updater.TryUpdateAll(cancellation_token).GetAwaiter().GetResult();

    It should_fail = () => result.Success.ShouldBeFalse();
    It should_fail_with_the_first_error = () => result.Exception.ShouldBeTheSameAs(exception);
    It should_ask_the_embedding_store_for_keys = () => embedding_store.Verify(_ => _.TryGetKeys(embedding, cancellation_token));
    It should_get_the_last_state_from_the_embedding_store_for_a = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_a, cancellation_token));
    It should_ask_the_event_store_for_new_events_for_a = () => event_store.Verify(_ => _.FetchForAggregateAfter(projection_key_a.Value, embedding.Value, current_state_a.Version, cancellation_token));
    It should_project_the_event_for_a = () => project_many_events.Verify(_ => _.TryProject(current_state_a, unprocessed_events_a, cancellation_token));
    It should_delete_for_a = () => embedding_store.Verify(_ => _.TryRemove(embedding, projection_key_a, projection_result_a.Version, cancellation_token));
    It should_get_the_last_state_from_the_embedding_store_for_b = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_b, cancellation_token));
    It should_ask_the_event_store_for_new_events_for_b = () => event_store.Verify(_ => _.FetchForAggregateAfter(projection_key_b.Value, embedding.Value, current_state_b.Version, cancellation_token));
    It should_project_the_event_for_b = () => project_many_events.Verify(_ => _.TryProject(current_state_b, unprocessed_events_b, cancellation_token));
    It should_replace_for_b = () => embedding_store.Verify(_ => _.TryReplace(embedding, projection_key_b, projection_result_b.Version, projection_result_b.State, cancellation_token));
}