// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_StateTransitionEventsCalculator.when_converging;

public class and_projecting_events_fails : given.all_dependencies
{
    static EmbeddingCurrentState current_state;
    static ProjectionState desired_state;
    static UncommittedEvents events;

    Establish context = () =>
    {
        current_state = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.CreatedFromInitialState, "current state", "");
        desired_state = "desired state";
        events = new UncommittedEvents(Array.Empty<UncommittedEvent>());
        state_comparer
            .Setup(_ => _.TryCheckEquality(current_state.State, desired_state))
            .Returns(Try<bool>.Succeeded(false));
        embedding
            .Setup(_ => _.TryCompare(current_state, desired_state, execution_context, cancellation))
            .Returns(Task.FromResult(Try<UncommittedEvents>.Succeeded(events)));
        project_many_events
            .Setup(_ => _.TryProject(current_state, events, execution_context, cancellation))
            .Returns(Task.FromResult(Partial<EmbeddingCurrentState>.Failed(new Exception())));
    };

    static Try<UncommittedAggregateEvents> result;
    Because of = () => result = calculator.TryConverge(current_state, desired_state, execution_context, cancellation).GetAwaiter().GetResult();

    It should_return_a_failure = () => result.Success.Should().BeFalse();
    It should_fail_because_detecting_loop_failed = () => result.Exception.Should().BeOfType<FailedProjectingEvents>();
    It should_only_compare_once = () => embedding.Verify(_ => _.TryCompare(current_state, desired_state, execution_context, cancellation), Moq.Times.Once);
    It should_not_do_anything_more_with_embedding = () => embedding.VerifyNoOtherCalls();
    It should_project_events = () => project_many_events.Verify(_ => _.TryProject(current_state, events, execution_context, cancellation), Moq.Times.Once);
    It should_not_project_anything_else = () => project_many_events.VerifyNoOtherCalls();
}