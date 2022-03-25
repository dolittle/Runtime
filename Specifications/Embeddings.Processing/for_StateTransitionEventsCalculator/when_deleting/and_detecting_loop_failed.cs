// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_StateTransitionEventsCalculator.when_deleting;

public class and_detecting_loop_failed : given.all_dependencies
{
    static EmbeddingCurrentState current_state;
    static UncommittedEvents events;
    static EmbeddingCurrentState desired_current_embedding_state;
    static DetectingEmbeddingLoopFailed exception;

    Establish context = () =>
    {
        exception = new DetectingEmbeddingLoopFailed();
        current_state = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.CreatedFromInitialState, "current state", "");
        desired_current_embedding_state = new EmbeddingCurrentState(1, EmbeddingCurrentStateType.Deleted, "", "");
        events = new UncommittedEvents(Array.Empty<UncommittedEvent>());
        embedding
            .Setup(_ => _.TryDelete(current_state, execution_context, cancellation))
            .Returns(Task.FromResult(Try<UncommittedEvents>.Succeeded(events)));
        project_many_events
            .Setup(_ => _.TryProject(current_state, events, execution_context, cancellation))
            .Returns(Task.FromResult(Partial<EmbeddingCurrentState>.Succeeded(
                desired_current_embedding_state)));
        loop_detector
            .Setup(_ => _.TryCheckForProjectionStateLoop(desired_current_embedding_state.State, new[] { current_state.State }))
            .Returns(exception);
    };

    static Try<UncommittedAggregateEvents> result;
    Because of = () => result = calculator.TryDelete(current_state, execution_context, cancellation).GetAwaiter().GetResult();

    It should_return_a_failure = () => result.Success.ShouldBeFalse();
    It should_fail_because_detecting_loop_failed = () => result.Exception.ShouldEqual(exception);
    It should_only_delete_once = () => embedding.Verify(_ => _.TryDelete(current_state, execution_context, cancellation), Moq.Times.Once);
    It should_not_do_anything_more_with_embedding = () => embedding.VerifyNoOtherCalls();
    It should_project_events = () => project_many_events.Verify(_ => _.TryProject(current_state, events, execution_context, cancellation), Moq.Times.Once);
    It should_not_project_anything_else = () => project_many_events.VerifyNoOtherCalls();
}