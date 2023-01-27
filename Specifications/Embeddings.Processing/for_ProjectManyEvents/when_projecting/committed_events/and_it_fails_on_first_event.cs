// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_ProjectManyEvents.when_projecting.committed_events;

public class and_it_fails_on_first_event : given.all_dependencies
{
    static ProjectionKey projection_key;
    static EventSourceId event_source;
    static Artifact aggregate_root_type;
    static EmbeddingCurrentState current_state;
    static CommittedAggregateEvent event_one;
    static CommittedAggregateEvent event_two;
    static CommittedAggregateEvents unprocessed_events;
    static ProjectionFailedResult result_after_one;
    static Exception exception;

    Establish context = () =>
    {
        projection_key = "projection-key";
        event_source = "642ca1f2-c8e1-4e5c-a213-246ce95a8376";
        aggregate_root_type = new Artifact(identifier.Value, ArtifactGeneration.First);
        current_state = new EmbeddingCurrentState(3, EmbeddingCurrentStateType.Persisted, "state-current", projection_key);
        event_one = new CommittedAggregateEvent(aggregate_root_type, 3, 10, DateTimeOffset.Now, event_source, execution_context, event_type, false, "event-one-content");
        event_two = new CommittedAggregateEvent(aggregate_root_type, 4, 11, DateTimeOffset.Now, event_source, execution_context, event_type, true, "event-two-content");
        unprocessed_events = new CommittedAggregateEvents(event_source, aggregate_root_type.Id, current_state.Version, new[] { event_one, event_two });
        exception = new Exception();
        result_after_one = new ProjectionFailedResult(exception);

        embedding
            .Setup(_ => _.Project(current_state, Moq.It.Is<UncommittedEvent>(_ => _.Content == event_one.Content), execution_context, cancellation_token))
            .Returns(Task.FromResult<IProjectionResult>(result_after_one));
    };

    static Partial<EmbeddingCurrentState> result;
    Because of = () => result = project_many_events.TryProject(current_state, unprocessed_events, execution_context, cancellation_token).GetAwaiter().GetResult();

    It should_fail = () => result.Success.Should().BeFalse();
    It should_not_be_a_partial_success = () => result.IsPartialResult.Should().BeFalse();
    It should_fail_with_the_correct_error = () => result.Exception.Should().Be(exception);
    It should_project_the_first_event = () => embedding.Verify(_ => _.Project(current_state, Moq.It.Is<UncommittedEvent>(_ => _.Content == event_one.Content), execution_context, cancellation_token));
    It should_only_project_the_first_event = () => embedding.VerifyNoOtherCalls();
}