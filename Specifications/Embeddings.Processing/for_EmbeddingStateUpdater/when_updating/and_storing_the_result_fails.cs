// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingStateUpdater.when_updating;

public class and_storing_the_result_fails : given.all_dependencies
{
    static ProjectionKey projection_key;
    static EmbeddingCurrentState current_state;
    static CommittedAggregateEvent committed_event;
    static CommittedAggregateEvents unprocessed_events;
    static EmbeddingCurrentState projection_result;
    static Exception exception;

    Establish context = () =>
    {
        projection_key = "projection-key";
        current_state = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.Persisted, "state-current", projection_key);
        committed_event = new CommittedAggregateEvent(new Artifact(embedding.Value, ArtifactGeneration.First), 3, 10, DateTimeOffset.Now, projection_key.Value, execution_context, event_type, false, "event-one-content");
        unprocessed_events = new CommittedAggregateEvents(projection_key.Value, embedding.Value, current_state.Version, new[] { committed_event });
        projection_result = new EmbeddingCurrentState(current_state.Version + 1, EmbeddingCurrentStateType.Persisted, current_state.State, current_state.Key);
        exception = new Exception();

        embedding_store.Setup(_ => _.TryGetKeys(embedding, cancellation_token)).Returns(Task.FromResult<Try<IEnumerable<ProjectionKey>>>(new[] { projection_key }));
        embedding_store.Setup(_ => _.TryGet(embedding, projection_key, cancellation_token)).Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state));
        embedding_store.Setup(_ => _.TryReplace(embedding, projection_key, projection_result.Version, projection_result.State, cancellation_token)).Returns(Task.FromResult<Try>(exception));
        committed_events_fetcher.Setup(_ => _.FetchForAggregateAfter(projection_key.Value, embedding.Value, current_state.Version, cancellation_token)).Returns(Task.FromResult(unprocessed_events));
        project_many_events.Setup(_ => _.TryProject(current_state, unprocessed_events, execution_context, cancellation_token)).Returns(Task.FromResult<Partial<EmbeddingCurrentState>>(projection_result));
    };

    static Try result;
    Because of = () => result = state_updater.TryUpdateAll(execution_context, cancellation_token).GetAwaiter().GetResult();

    It should_fail = () => result.Success.ShouldBeFalse();
    It should_fail_with_the_correct_error = () => result.Exception.ShouldBeTheSameAs(exception);
    It should_ask_the_embedding_store_for_keys = () => embedding_store.Verify(_ => _.TryGetKeys(embedding, cancellation_token));
    It should_get_the_last_state_from_the_embedding_store = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key, cancellation_token));
    It should_ask_the_event_store_for_new_events = () => committed_events_fetcher.Verify(_ => _.FetchForAggregateAfter(projection_key.Value, embedding.Value, current_state.Version, cancellation_token));
    It should_project_the_event = () => project_many_events.Verify(_ => _.TryProject(current_state, unprocessed_events, execution_context, cancellation_token));
    It should_store_the_final_state = () => embedding_store.Verify(_ => _.TryReplace(embedding, projection_key, projection_result.Version, projection_result.State, cancellation_token));
}