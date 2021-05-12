// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
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
    public class and_projecting_fails : given.all_dependencies
    {
        static ProjectionKey projection_key;
        static EventSourceId event_source;
        static EmbeddingCurrentState current_state;
        static CommittedAggregateEvent unprocessed_event;
        static CommittedAggregateEvents unprocessed_events;
        static Exception exception;

        Establish context = () =>
        {
            projection_key = "projection-key";
            event_source = Guid.Parse("642ca1f2-c8e1-4e5c-a213-246ce95a8376");
            current_state = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.Persisted, "state-current", projection_key);
            unprocessed_event = new CommittedAggregateEvent(new Artifact(embedding.Value, ArtifactGeneration.First), 3, 10, DateTimeOffset.Now, event_source, execution_context, event_type, false, "event-one-content");
            unprocessed_events = new CommittedAggregateEvents(event_source, embedding.Value, new[] { unprocessed_event });
            exception = new Exception();

            embedding_store
                .Setup(_ => _.TryGetKeys(embedding, cancellation_token))
                .Returns(Task.FromResult<Try<IEnumerable<ProjectionKey>>>(new[] { projection_key }));
            key_converter
                .Setup(_ => _.GetEventSourceIdFor(projection_key))
                .Returns(event_source);
            embedding_store
                .Setup(_ => _.TryGet(embedding, projection_key, cancellation_token))
                .Returns(Task.FromResult<Try<EmbeddingCurrentState>>(current_state));
            event_store
                .Setup(_ => _.FetchForAggregateAfter(event_source, embedding.Value, current_state.Version, cancellation_token))
                .Returns(Task.FromResult(unprocessed_events));
            project_many_events
                .Setup(_ => _.TryProject(current_state, unprocessed_events, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Partial<EmbeddingCurrentState>>(exception));
        };

        static Try result;
        Because of = () => result = state_updater.TryUpdateAll(cancellation_token).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_with_the_correct_error = () => result.Exception.ShouldEqual(exception);
        It should_ask_the_embedding_store_for_keys = () => embedding_store.Verify(_ => _.TryGetKeys(embedding, cancellation_token));
        It should_get_the_last_state_from_the_embedding_store = () => embedding_store.Verify(_ => _.TryGet(embedding, projection_key, cancellation_token));
        It should_ask_the_event_store_for_new_events = () => event_store.Verify(_ => _.FetchForAggregateAfter(event_source, embedding.Value, current_state.Version, cancellation_token));
        It should_project_the_events = () => project_many_events.Verify(_ => _.TryProject(current_state, unprocessed_events, cancellation_token));
        It should_not_store_any_state = () => embedding_store.VerifyNoOtherCalls();
    }
}
