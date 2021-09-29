// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_StateTransitionEventsCalculator.when_deleting
{
    public class and_projecting_events_fails : given.all_dependencies
    {
        static EmbeddingCurrentState current_state;
        static UncommittedEvents events;

        Establish context = () =>
        {
            current_state = new EmbeddingCurrentState(0, EmbeddingCurrentStateType.CreatedFromInitialState, "current state", "");
            events = new UncommittedEvents(Array.Empty<UncommittedEvent>());
            embedding
                .Setup(_ => _.TryDelete(current_state, cancellation))
                .Returns(Task.FromResult(Try<UncommittedEvents>.Succeeded(events)));
            project_many_events
                .Setup(_ => _.TryProject(current_state, events, cancellation))
                .Returns(Task.FromResult(Partial<EmbeddingCurrentState>.Failed(new Exception())));
        };

        static Try<UncommittedAggregateEvents> result;
        Because of = () => result = calculator.TryDelete(current_state, cancellation).GetAwaiter().GetResult();

        It should_return_a_failure = () => result.Success.ShouldBeFalse();
        It should_fail_because_detecting_loop_failed = () => result.Exception.ShouldBeOfExactType<FailedProjectingEvents>();
        It should_only_delete_once = () => embedding.Verify(_ => _.TryDelete(current_state, cancellation), Moq.Times.Once);
        It should_not_do_anything_more_with_embedding = () => embedding.VerifyNoOtherCalls();
        It should_project_events = () => project_many_events.Verify(_ => _.TryProject(current_state, events, cancellation), Moq.Times.Once);
        It should_not_project_anything_else = () => project_many_events.VerifyNoOtherCalls();
    }
}