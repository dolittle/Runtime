// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_deleting;

public class and_everything_works : given.all_dependencies_and_a_key
{
    static Task task;
    static UncommittedAggregateEvents uncommitted_events;
    static CommitAggregateEventsRequest commit_request;

    Establish context = () =>
    {
        uncommitted_events = CreateUncommittedEvents(uncommitted_event);
        commit_request = uncommitted_events.ToCommitRequest(execution_context);
        task = embedding_processor.Start(cancellation_token);
        embedding_store
            .Setup(_ => _.TryGet(embedding, key, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<EmbeddingCurrentState>.Succeeded(current_state)));
        transition_calculator
            .Setup(_ => _.TryDelete(current_state, execution_context, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<UncommittedAggregateEvents>.Succeeded(uncommitted_events)));
        event_store
            .Setup(_ => _.CommitAggregateEvents(commit_request, Moq.It.IsAny<CancellationToken>()))
            .ReturnsAsync(SuccessfulCommitResponse(committed_events));
        embedding_store
            .Setup(_ => _.TryRemove(embedding, key, aggregate_root_version, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try.Succeeded()));
    };

    static Try result;

    Because of = () => result = embedding_processor.Delete(key, execution_context, cancellation_token).GetAwaiter().GetResult();

    It should_still_be_running = () => task.Status.Should().Be(TaskStatus.WaitingForActivation);
    It should_fetch_the_current_state = () => embedding_store.Verify(_ => _.TryGet(embedding, key, Moq.It.IsAny<CancellationToken>()));
    It should_calculate_the_transition_events = () => transition_calculator.Verify(_ => _.TryDelete(current_state, execution_context, Moq.It.IsAny<CancellationToken>()));
    It should_commit_the_calculated_events = () => event_store.Verify(_ => _.CommitAggregateEvents(commit_request, Moq.It.IsAny<CancellationToken>()));
    It should_remove_the_state = () => embedding_store.Verify(_ => _.TryRemove(embedding, key, aggregate_root_version, Moq.It.IsAny<CancellationToken>()));
    It should_return_success = () => result.Success.Should().BeTrue();
}