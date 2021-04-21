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
    public class and_two_of_three_keys_fails : given.all_dependencies
    {
        static Artifact aggregate_root_type;
        static Exception exception_a;
        static Exception exception_b;
        static ProjectionKey projection_key_a;
        static EventSourceId event_source_a;
        static EmbeddingCurrentState current_state_a;
        static CommittedAggregateEvent event_a;
        static CommittedAggregateEvents unprocessed_events_a;
        static ProjectionDeleteResult result_a;
        static ProjectionKey projection_key_b;
        static EventSourceId event_source_b;
        static ProjectionKey projection_key_c;
        static EventSourceId event_source_c;
        static EmbeddingCurrentState current_state_c;
        static CommittedAggregateEvent event_c;
        static CommittedAggregateEvents unprocessed_events_c;
        static ProjectionReplaceResult result_c;

        Establish context = () =>
        {
            aggregate_root_type = new Artifact(embedding.Value, ArtifactGeneration.First);
            exception_a = new Exception();
            exception_b = new Exception();

            projection_key_a = "key-a";
            event_source_a = Guid.Parse("f0cbe001-65a4-49d2-adcb-ff4d42e92223");
            current_state_a = new EmbeddingCurrentState(1, ProjectionCurrentStateType.Persisted, "state-current-a", projection_key_a);
            event_a = new CommittedAggregateEvent(aggregate_root_type, 1, 1, DateTimeOffset.Now, event_source_a, execution_context, event_type, false, "event-a-content");
            unprocessed_events_a = new CommittedAggregateEvents(event_source_a, aggregate_root_type.Id, new[] { event_a });
            result_a = new ProjectionDeleteResult();

            projection_key_b = "key-b";
            event_source_b = Guid.Parse("760a14d6-639d-4eba-9e7e-48675631bdd2");

            projection_key_c = "key-c";
            event_source_c = Guid.Parse("313ef6a7-62da-4761-b054-fd33d89d5ebd");
            current_state_c = new EmbeddingCurrentState(3, ProjectionCurrentStateType.CreatedFromInitialState, "state-initial-c", projection_key_c);
            event_c = new CommittedAggregateEvent(aggregate_root_type, 3, 10, DateTimeOffset.Now, event_source_c, execution_context, event_type, true, "event-c-content");
            unprocessed_events_c = new CommittedAggregateEvents(event_source_c, aggregate_root_type.Id, new[] { event_c });
            result_c = new ProjectionReplaceResult("state-after-c");

            embedding_store.Setup(_ => _.TryGetKeys(embedding, cancellation_token)).Returns(Task.FromResult<Try<IEnumerable<ProjectionKey>>>(new[] { projection_key_a, projection_key_b, projection_key_c }));

            key_converter.Setup(_ => _.GetEventSourceIdFor(projection_key_a)).Returns(event_source_a);
            embedding_store.Setup(_ => _.TryGet(embedding, projection_key_a, cancellation_token)).Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state_a));
            event_store.Setup(_ => _.FetchForAggregateAfter(event_source_a, embedding.Value, current_state_a.Version, cancellation_token)).Returns(Task.FromResult(unprocessed_events_a));
            projection.Setup(_ => _.Project(current_state_a, event_a, events_partition, cancellation_token)).Returns(Task.FromResult<IProjectionResult>(result_a));
            embedding_store.Setup(_ => _.TryRemove(embedding, projection_key_a, event_a.AggregateRootVersion + 1, cancellation_token)).Returns(Task.FromResult<Try>(exception_a));

            key_converter.Setup(_ => _.GetEventSourceIdFor(projection_key_b)).Returns(event_source_b);
            embedding_store.Setup(_ => _.TryGet(embedding, projection_key_b, cancellation_token)).Returns(Task.FromResult<Try<EmbeddingCurrentState>>(exception_b));

            key_converter.Setup(_ => _.GetEventSourceIdFor(projection_key_c)).Returns(event_source_c);
            embedding_store.Setup(_ => _.TryGet(embedding, projection_key_c, cancellation_token)).Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state_c));
            event_store.Setup(_ => _.FetchForAggregateAfter(event_source_c, embedding.Value, current_state_c.Version, cancellation_token)).Returns(Task.FromResult(unprocessed_events_c));
            projection.Setup(_ => _.Project(current_state_c, event_c, events_partition, cancellation_token)).Returns(Task.FromResult<IProjectionResult>(result_c));
            embedding_store.Setup(_ => _.TryReplace(embedding, projection_key_c, event_c.AggregateRootVersion + 1, result_c.State, cancellation_token)).Returns(Task.FromResult<Try>(true));
        };

        static Try result;
        Because of = () => result = state_updater.TryUpdateAll(cancellation_token).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_with_the_first_error = () => result.Exception.ShouldBeTheSameAs(exception_a);
        It should_ask_the_embedding_store_for_keys = () => embedding_store.Verify(_ => _.TryGetKeys(embedding, cancellation_token));
        It should_get_the_last_state_from_the_embedding_store_for_a = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_a, cancellation_token));
        It should_ask_the_event_store_for_new_events_for_a = () => event_store.Verify(_ => _.FetchForAggregateAfter(event_source_a, embedding.Value, current_state_a.Version, cancellation_token));
        It should_project_the_event_for_a = () => projection.Verify(_ => _.Project(current_state_a, event_a, events_partition, cancellation_token));
        It should_delete_for_a = () => embedding_store.Verify(_ => _.TryRemove(embedding, projection_key_a, event_a.AggregateRootVersion + 1, cancellation_token));
        It should_get_the_last_state_from_the_embedding_store_for_b = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_b, cancellation_token));
        It should_get_the_last_state_from_the_embedding_store_for_c = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key_c, cancellation_token));
        It should_ask_the_event_store_for_new_events_for_c = () => event_store.Verify(_ => _.FetchForAggregateAfter(event_source_c, embedding.Value, current_state_c.Version, cancellation_token));
        It should_project_the_event_for_c = () => projection.Verify(_ => _.Project(current_state_c, event_c, events_partition, cancellation_token));
        It should_update_for_c = () => embedding_store.Verify(_ => _.TryReplace(embedding, projection_key_c, event_c.AggregateRootVersion + 1, result_c.State, cancellation_token));
    }
}
