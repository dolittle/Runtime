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

namespace Dolittle.Runtime.Embeddings.Processing.for_ProjectManyEvents.when_projecting.uncommitted_events
{
    public class and_everything_works : given.all_dependencies
    {
        static ProjectionKey projection_key;
        static EventSourceId event_source;
        static EmbeddingCurrentState current_state;
        static UncommittedEvent event_one;
        static UncommittedEvent event_two;
        static UncommittedEvent event_three;
        static UncommittedEvents unprocessed_events;
        static ProjectionReplaceResult result_after_one;
        static ProjectionReplaceResult result_after_two;
        static ProjectionDeleteResult result_after_three;

        Establish context = () =>
        {
            projection_key = "projection-key-ZSF";
            event_source = Guid.Parse("ed882f2f-9974-4334-ad43-e662d6db3396");
            current_state = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.Persisted, "state-current", projection_key);
            event_one = new UncommittedEvent(event_source, event_type, false, "event-one-content");
            event_two = new UncommittedEvent(event_source, event_type, true, "event-two-content");
            event_three = new UncommittedEvent(event_source, event_type, false, "event-three-content");
            unprocessed_events = new UncommittedEvents(new[] { event_one, event_two, event_three });
            result_after_one = new ProjectionReplaceResult("state-after-one");
            result_after_two = new ProjectionReplaceResult("state-after-two");
            result_after_three = new ProjectionDeleteResult();
            embedding
                .Setup(_ => _.Project(current_state, event_one, cancellation_token))
                .Returns(Task.FromResult<IProjectionResult>(result_after_one));
            embedding
                .Setup(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_one.State.Value), event_two, cancellation_token))
                .Returns(Task.FromResult<IProjectionResult>(result_after_two));
            embedding
                .Setup(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_two.State.Value), event_three, cancellation_token))
                .Returns(Task.FromResult<IProjectionResult>(result_after_three));
        };

        static Partial<EmbeddingCurrentState> result;
        Because of = () => result = project_many_events.TryProject(current_state, unprocessed_events, cancellation_token).GetAwaiter().GetResult();

        It should_succeed = () => result.Success.ShouldBeTrue();
        It should_return_a_deleted_state = () => result.Result.Type.ShouldEqual(EmbeddingCurrentStateType.Deleted);
        It should_return_the_same_key = () => result.Result.Key.ShouldEqual(projection_key);
        It should_return_the_correct_aggregate_root_version = () => result.Result.Version.Value.ShouldEqual(current_state.Version.Value + 3);
        It should_project_the_first_event = () => embedding.Verify(_ => _.Project(current_state, event_one, cancellation_token));
        It should_project_the_second_event = () => embedding.Verify(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_one.State.Value), event_two, cancellation_token));
        It should_project_the_third_event = () => embedding.Verify(_ => _.Project(Moq.It.Is<ProjectionCurrentState>(_ => _.State.Value == result_after_two.State.Value), event_three, cancellation_token));
    }
}
