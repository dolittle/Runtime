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
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_updating
{
    public class and_updating_the_state_fails : given.all_dependencies_and_a_desired_state
    {
        static Task task;
        static Exception exception;
        static UncommittedAggregateEvents uncommitted_events;
        Establish context = () =>
        {
            uncommitted_events = CreateUncommittedEvents(uncommitted_event);
            exception = new Exception();
            task = embedding_processor.Start(cancellation_token);
            embedding_store
                .Setup(_ => _.TryGet(embedding, key, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingCurrentState>.Succeeded(current_state)));
            transition_calculator
                .Setup(_ => _.TryConverge(current_state, desired_state, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<UncommittedAggregateEvents>.Succeeded(uncommitted_events)));
            event_store
                .Setup(_ => _.CommitAggregateEvents(uncommitted_events, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(committed_events));
            embedding_store
                .Setup(_ => _.TryReplace(embedding, key, aggregate_root_version, desired_state, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try.Failed(exception)));
        };

        static Try<ProjectionState> result;

        Because of = () => result = embedding_processor.Update(key, desired_state, Moq.It.IsAny<CancellationToken>()).GetAwaiter().GetResult();

        It should_still_be_running = () => task.Status.ShouldEqual(TaskStatus.WaitingForActivation);
        It should_fetch_the_current_state = () => embedding_store.Verify(_ => _.TryGet(embedding, key, Moq.It.IsAny<CancellationToken>()));
        It should_calculate_the_transition_events = () => transition_calculator.Verify(_ => _.TryConverge(current_state, desired_state, Moq.It.IsAny<CancellationToken>()));
        It should_commit_the_calculated_events = () => event_store.Verify(_ => _.CommitAggregateEvents(uncommitted_events, Moq.It.IsAny<CancellationToken>()));
        It should_store_the_updated_state = () => embedding_store.Verify(_ => _.TryReplace(embedding, key, aggregate_root_version, desired_state, Moq.It.IsAny<CancellationToken>()));
        It should_return_the_desired_state = () => result.Result.ShouldEqual(desired_state);
    }
}
