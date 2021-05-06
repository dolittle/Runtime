// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_StateTransitionEventsCalculator.when_converging
{
    public class and_comparing_states_fails : given.all_dependencies
    {
        static EmbeddingCurrentState current_state;
        static ProjectionState desired_state;
        static FailedProjectingEvents exception;
        static EmbeddingCurrentState desired_current_embedding_state;
        static UncommittedEvents events;

        Establish context = () =>
        {
            exception = new FailedProjectingEvents(identifier, new Exception());
            current_state = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.CreatedFromInitialState, "current state", "");
            desired_state = "desired state";
            desired_current_embedding_state = new EmbeddingCurrentState(1, EmbeddingCurrentStateType.Persisted, desired_state, "");
            events = new UncommittedEvents(Array.Empty<UncommittedEvent>());
            embedding
                .Setup(_ => _.TryCompare(current_state, desired_state, cancellation))
                .Returns(Task.FromResult(Try<UncommittedEvents>.Succeeded(events)));
            loop_detector
                .Setup(_ => _.TryCheckEventLoops(new[] { events }))
                .Returns(Task.FromResult(Try<bool>.Succeeded(false)));
            project_many_events
                .Setup(_ => _.TryProject(current_state, events, cancellation))
                .Returns(Task.FromResult(Partial<EmbeddingCurrentState>.Succeeded(
                    desired_current_embedding_state)));
            state_comparer
                .Setup(_ => _.TryCheckEquality(current_state.State, desired_state))
                .Returns(Try<bool>.Failed(exception));
            state_comparer
                .Setup(_ => _.TryCheckEquality(desired_current_embedding_state.State, desired_state))
                .Returns(Try<bool>.Failed(exception));
        };

        static Try<UncommittedAggregateEvents> result;
        Because of = () => result = calculator.TryConverge(current_state, desired_state, cancellation).GetAwaiter().GetResult();

        It should_return_a_failure = () => result.Success.ShouldBeFalse();
        It should_fail_because_comparing_states_failed = () => result.Exception.ShouldEqual(exception);
        It should_not_do_anything_with_embedding = () => embedding.VerifyNoOtherCalls();
        It should_not_project_anything = () => project_many_events.VerifyNoOtherCalls();
    }
}