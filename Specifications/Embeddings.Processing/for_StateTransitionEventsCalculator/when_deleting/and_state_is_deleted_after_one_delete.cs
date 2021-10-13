// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_StateTransitionEventsCalculator.when_deleting
{
    public class and_desired_state_is_reached_after_one_compare : given.all_dependencies
    {
        static EmbeddingCurrentState current_state;
        static EmbeddingCurrentState desired_current_embedding_state;
        static UncommittedEvents events;

        Establish context = () =>
        {
            current_state = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.CreatedFromInitialState, "current state", "");
            desired_current_embedding_state = new EmbeddingCurrentState(1, EmbeddingCurrentStateType.Deleted, "", "");
            events = new UncommittedEvents(Array.Empty<UncommittedEvent>());
            embedding
                .Setup(_ => _.TryDelete(current_state, cancellation))
                .Returns(Task.FromResult(Try<UncommittedEvents>.Succeeded(events)));
            loop_detector
                .Setup(_ => _.TryCheckForProjectionStateLoop(desired_current_embedding_state.State, new[] { current_state.State }))
                .Returns(false);
            project_many_events
                .Setup(_ => _.TryProject(current_state, events, cancellation))
                .Returns(Task.FromResult(Partial<EmbeddingCurrentState>.Succeeded(
                    desired_current_embedding_state)));
        };

        static Try<UncommittedAggregateEvents> result;
        Because of = () => result = calculator.TryDelete(current_state, cancellation).GetAwaiter().GetResult();

        It should_not_return_a_failure = () => result.Success.ShouldBeTrue();
        It should_only_delete_once = () => embedding.Verify(_ => _.TryDelete(current_state, cancellation), Moq.Times.Once);
        It should_not_do_anything_more_with_embedding = () => embedding.VerifyNoOtherCalls();
        It should_project_events = () => project_many_events.Verify(_ => _.TryProject(current_state, events, cancellation), Moq.Times.Once);
        It should_not_project_anything_else = () => project_many_events.VerifyNoOtherCalls();
        It should_return_the_same_events = () => result.Result.ShouldContainOnly(events);
        It should_return_uncommitted_events_with_correct_aggregate_root_id = () => result.Result.AggregateRoot.Id.Value.ShouldEqual(identifier.Value);
        It should_return_uncommitted_events_with_correct_event_source_id = () => result.Result.EventSource.Value.ShouldEqual(current_state.Key.Value);
        It should_return_uncommitted_events_with_correct_aggregate_root_version = () => result.Result.ExpectedAggregateRootVersion.Value.ShouldEqual<ulong>(1);
    }
}