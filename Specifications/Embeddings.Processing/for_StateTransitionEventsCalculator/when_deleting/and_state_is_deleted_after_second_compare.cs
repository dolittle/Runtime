// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_StateTransitionEventsCalculator.when_deleting
{
    public class and_desired_state_is_reached_after_second_compare : given.all_dependencies
    {
        static EmbeddingCurrentState current_state;
        static EmbeddingCurrentState intermediate_current_embedding_state;
        static EmbeddingCurrentState desired_current_embedding_state;
        static UncommittedEvents first_event_batch;
        static UncommittedEvents second_event_batch;

        Establish context = () =>
        {
            current_state = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.CreatedFromInitialState, "current state", "");
            intermediate_current_embedding_state = new EmbeddingCurrentState(1, EmbeddingCurrentStateType.Persisted, "intermediate state", "");
            desired_current_embedding_state = new EmbeddingCurrentState(2, EmbeddingCurrentStateType.Deleted, "", "");
            first_event_batch = new UncommittedEvents(Array.Empty<UncommittedEvent>());
            second_event_batch = new UncommittedEvents(Array.Empty<UncommittedEvent>());
            embedding
                .Setup(_ => _.TryDelete(current_state, cancellation))
                .Returns(Task.FromResult(Try<UncommittedEvents>.Succeeded(first_event_batch)));
            embedding
                .Setup(_ => _.TryDelete(intermediate_current_embedding_state, cancellation))
                .Returns(Task.FromResult(Try<UncommittedEvents>.Succeeded(second_event_batch)));
            loop_detector
                .Setup(_ => _.TryCheckForProjectionStateLoop(intermediate_current_embedding_state.State, new[] { current_state.State }))
                .Returns(false);
            loop_detector
                .Setup(_ => _.TryCheckForProjectionStateLoop(desired_current_embedding_state.State, new[] { current_state.State, intermediate_current_embedding_state.State }))
                .Returns(false);
            project_many_events
                .Setup(_ => _.TryProject(current_state, first_event_batch, cancellation))
                .Returns(Task.FromResult(Partial<EmbeddingCurrentState>.Succeeded(
                    intermediate_current_embedding_state)));
            project_many_events
                .Setup(_ => _.TryProject(intermediate_current_embedding_state, second_event_batch, cancellation))
                .Returns(Task.FromResult(Partial<EmbeddingCurrentState>.Succeeded(
                    desired_current_embedding_state)));
        };

        static Try<UncommittedAggregateEvents> result;
        Because of = () => result = calculator.TryDelete(current_state, cancellation).GetAwaiter().GetResult();

        It should_not_return_a_failure = () => result.Success.ShouldBeTrue();
        It should_delete_with_current_state = () => embedding.Verify(_ => _.TryDelete(current_state, cancellation), Moq.Times.Once);
        It should_delete_with_intermediate_state = () => embedding.Verify(_ => _.TryDelete(intermediate_current_embedding_state, cancellation), Moq.Times.Once);
        It should_not_do_anything_more_with_embedding = () => embedding.VerifyNoOtherCalls();
        It should_project_first_batch_of_events = () => project_many_events.Verify(_ => _.TryProject(current_state, first_event_batch, cancellation), Moq.Times.Once);
        It should_project_second_batch_of_events = () => project_many_events.Verify(_ => _.TryProject(intermediate_current_embedding_state, second_event_batch, cancellation), Moq.Times.Once);
        It should_not_project_anything_else = () => project_many_events.VerifyNoOtherCalls();
        It should_return_all_the_same_events = () => result.Result.ShouldContainOnly(first_event_batch.Concat(second_event_batch));
        It should_return_uncommitted_events_with_correct_aggregate_root_id = () => result.Result.AggregateRoot.Id.Value.ShouldEqual(identifier.Value);
        It should_return_uncommitted_events_with_correct_event_source_id = () => result.Result.EventSource.ShouldEqual(key_to_event_source_converter.GetEventSourceIdFor(current_state.Key));
        It should_return_uncommitted_events_with_correct_aggregate_root_version = () => result.Result.ExpectedAggregateRootVersion.Value.ShouldEqual<ulong>(2);
    }
}