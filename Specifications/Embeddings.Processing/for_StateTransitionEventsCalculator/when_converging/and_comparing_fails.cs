// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_StateTransitionEventsCalculator.when_converging;

public class and_comparing_fails : given.all_dependencies
{
    static EmbeddingCurrentState current_state;
    static ProjectionState desired_state;
    static Exception exception;

    Establish context = () =>
    {
        exception = new Exception();
        current_state = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.CreatedFromInitialState, "current state", "");
        desired_state = "desired state";
        state_comparer
            .Setup(_ => _.TryCheckEquality(current_state.State, desired_state))
            .Returns(Try<bool>.Succeeded(false));
        embedding
            .Setup(_ => _.TryCompare(current_state, desired_state, execution_context, cancellation))
            .Returns(Task.FromResult(Try<UncommittedEvents>.Failed(exception)));
    };

    static Try<UncommittedAggregateEvents> result;
    Because of = () => result = calculator.TryConverge(current_state, desired_state, execution_context, cancellation).GetAwaiter().GetResult();

    It should_return_a_failure = () => result.Success.ShouldBeFalse();
    It should_fail_because_detecting_loop_failed = () => result.Exception.ShouldEqual(exception);
    It should_only_compare_once = () => embedding.Verify(_ => _.TryCompare(current_state, desired_state, execution_context, cancellation), Moq.Times.Once);
    It should_not_do_anything_more_with_embedding = () => embedding.VerifyNoOtherCalls();
    It should_not_project_any_events = () => project_many_events.VerifyNoOtherCalls();
}