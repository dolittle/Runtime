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
    public class two_keys : given.all_dependencies
    {
        static Artifact aggregate_root_type;
        static ProjectionKey projection_key_a;
        static EventSourceId event_source_a;
        static EmbeddingCurrentState current_state_a;
        static CommittedAggregateEvent event_a;
        static CommittedAggregateEvents unprocessed_events_a;
        static ProjectionDeleteResult result_a;
        static ProjectionKey projection_key_b;
        static EventSourceId event_source_b;
        static EmbeddingCurrentState current_state_b;
        static CommittedAggregateEvent event_b;
        static CommittedAggregateEvents unprocessed_events_b;
        static ProjectionReplaceResult result_b;

        Establish context = () =>
        {
            aggregate_root_type = new Artifact(embedding.Value, ArtifactGeneration.First);

            projection_key_a = "key-a";
            event_source_a = Guid.Parse("f0cbe001-65a4-49d2-adcb-ff4d42e92223");
            current_state_a = new EmbeddingCurrentState(1, ProjectionCurrentStateType.Persisted, "state-current-a", projection_key_a);
            event_a = new CommittedAggregateEvent(aggregate_root_type, 1, 1, DateTimeOffset.Now, event_source_a, execution_context, event_type, false, "event-a-content");
            unprocessed_events_a = new CommittedAggregateEvents(event_source_a, aggregate_root_type.Id, new[] { event_a });
            result_a = new ProjectionDeleteResult();

            projection_key_b = "key-b";
            event_source_b = Guid.Parse("760a14d6-639d-4eba-9e7e-48675631bdd2");
            current_state_b = new EmbeddingCurrentState(0, ProjectionCurrentStateType.CreatedFromInitialState, "state-initial-b", projection_key_b);
            event_b = new CommittedAggregateEvent(aggregate_root_type, 0, 2, DateTimeOffset.Now, event_source_b, execution_context, event_type, false, "event-b-content");
            unprocessed_events_b = new CommittedAggregateEvents(event_source_b, aggregate_root_type.Id, new[] { event_b });
            result_b = new ProjectionReplaceResult("state-after-b");

            embedding_store.Setup(_ => _.TryGetKeys(embedding, cancellation_token)).Returns(Task.FromResult<Try<IEnumerable<ProjectionKey>>>(new []{ projection_key_a, projection_key_b }));

            key_converter.Setup(_ => _.GetEventSourceIdFor(projection_key_a)).Returns(event_source_a);
            embedding_store.Setup(_ => _.TryGet(embedding, projection_key_a, cancellation_token)).Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state_a));
            event_store.Setup(_ => _.FetchForAggregateAfter(event_source_a, embedding.Value, current_state_a.Version, cancellation_token)).Returns(Task.FromResult(unprocessed_events_a));
            projection.Setup(_ => _.Project(current_state_a, event_a, events_partition, cancellation_token)).Returns(Task.FromResult<IProjectionResult>(result_a));
            embedding_store.Setup(_ => _.TryRemove(embedding, projection_key_a, event_a.AggregateRootVersion+1, cancellation_token)).Returns(Task.FromResult<Try>(true));

            key_converter.Setup(_ => _.GetEventSourceIdFor(projection_key_b)).Returns(event_source_b);
            embedding_store.Setup(_ => _.TryGet(embedding, projection_key_b, cancellation_token)).Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state_b));
            event_store.Setup(_ => _.FetchForAggregateAfter(event_source_b, embedding.Value, current_state_b.Version, cancellation_token)).Returns(Task.FromResult(unprocessed_events_b));
            projection.Setup(_ => _.Project(current_state_b, event_b, events_partition, cancellation_token)).Returns(Task.FromResult<IProjectionResult>(result_b));
            embedding_store.Setup(_ => _.TryReplace(embedding, projection_key_b, event_b.AggregateRootVersion+1, result_b.State, cancellation_token)).Returns(Task.FromResult<Try>(true));
        };

        static Try result;
        Because of = () => result = state_updater.TryUpdateAll(cancellation_token).GetAwaiter().GetResult();

        It should_succeed = () => result.Success.ShouldBeTrue();
        It should_ask_the_embedding_store_for_keys = () => embedding_store.Verify(_ => _.TryGetKeys(embedding, cancellation_token));
        It should_ask_the_converter_for_event_source_id_a = () => key_converter.Verify(_ => _.GetEventSourceIdFor(projection_key_a));
        It should_get_the_last_state_from_the_embedding_store_for_a = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_a, cancellation_token));
        It should_ask_the_event_store_for_new_events_for_a = () => event_store.Verify(_ => _.FetchForAggregateAfter(event_source_a, embedding.Value, current_state_a.Version, cancellation_token));
        It should_project_the_event_for_a = () => projection.Verify(_ => _.Project(current_state_a, event_a, events_partition, cancellation_token));
        It should_delete_for_a = () => embedding_store.Verify(_ => _.TryRemove(embedding, projection_key_a, event_a.AggregateRootVersion+1, cancellation_token));
        It should_ask_the_converter_for_event_source_id_b = () => key_converter.Verify(_ => _.GetEventSourceIdFor(projection_key_b));
        It should_get_the_last_state_from_the_embedding_store_for_b = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_b, cancellation_token));
        It should_ask_the_event_store_for_new_events_for_b = () => event_store.Verify(_ => _.FetchForAggregateAfter(event_source_b, embedding.Value, current_state_b.Version, cancellation_token));
        It should_project_the_event_for_b = () => projection.Verify(_ => _.Project(current_state_b, event_b, events_partition, cancellation_token));
        It should_replace_for_b = () => embedding_store.Verify(_ => _.TryReplace(embedding, projection_key_b, event_b.AggregateRootVersion+1, result_b.State, cancellation_token));
    }
}
