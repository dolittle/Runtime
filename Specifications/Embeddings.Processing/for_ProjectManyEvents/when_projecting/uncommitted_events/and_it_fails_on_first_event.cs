// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_ProjectManyEvents.when_projecting.uncommitted_events;

public class and_it_fails_on_first_event : given.all_dependencies
{
    static ProjectionKey projection_key;
    static EventSourceId event_source;
    static EmbeddingCurrentState current_state;
    static UncommittedEvent event_one;
    static UncommittedEvent event_two;
    static UncommittedEvents unprocessed_events;
    static ProjectionFailedResult result_after_one;
    static Exception exception;

    Establish context = () =>
    {
        projection_key = "projection-key";
        event_source = "642ca1f2-c8e1-4e5c-a213-246ce95a8376";
        current_state = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.Persisted, "state-current", projection_key);
        event_one = new UncommittedEvent(event_source, event_type, false, "event-one-content");
        event_two = new UncommittedEvent(event_source, event_type, true, "event-two-content");
        unprocessed_events = new UncommittedEvents(new[] { event_one, event_two });
        exception = new Exception();
        result_after_one = new ProjectionFailedResult(exception);

        embedding
            .Setup(_ => _.Project(current_state, event_one, execution_context, cancellation_token))
            .Returns(Task.FromResult<IProjectionResult>(result_after_one));
    };

    static Partial<EmbeddingCurrentState> result;
    Because of = () => result = project_many_events.TryProject(current_state, unprocessed_events, execution_context, cancellation_token).GetAwaiter().GetResult();

    It should_fail = () => result.Success.ShouldBeFalse();
    It should_not_be_a_partial_success = () => result.IsPartialResult.ShouldBeFalse();
    It should_fail_with_the_correct_error = () => result.Exception.ShouldEqual(exception);
    It should_project_the_first_event = () => embedding.Verify(_ => _.Project(current_state, event_one, execution_context, cancellation_token));
    It should_only_project_the_first_event = () => embedding.VerifyNoOtherCalls();
}