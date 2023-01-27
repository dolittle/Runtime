// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Contracts;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using It = Machine.Specifications.It;
using UncommittedAggregateEvents = Dolittle.Runtime.Events.Store.UncommittedAggregateEvents;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_deleting;

public class and_there_are_no_transition_events : given.all_dependencies_and_a_key
{
    static Task task;

    Establish context = () =>
    {
        task = embedding_processor.Start(cancellation_token);
        embedding_store
            .Setup(_ => _.TryGet(embedding, key, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<EmbeddingCurrentState>.Succeeded(current_state)));
        transition_calculator
            .Setup(_ => _.TryDelete(current_state, execution_context, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<UncommittedAggregateEvents>.Succeeded(CreateUncommittedEvents())));
    };

    static Try result;

    Because of = () => result = embedding_processor.Delete(key, execution_context, cancellation_token).GetAwaiter().GetResult();

    It should_still_be_running = () => task.Status.Should().Be(TaskStatus.WaitingForActivation);
    It should_fetch_the_current_state = () => embedding_store.Verify(_ => _.TryGet(embedding, key, Moq.It.IsAny<CancellationToken>()));
    It should_calculate_the_transition_events = () => transition_calculator.Verify(_ => _.TryDelete(current_state, execution_context, Moq.It.IsAny<CancellationToken>()));
    It should_not_commit_any_events = () => event_store.Verify(_ => _.CommitAggregateEvents(Moq.It.IsAny<CommitAggregateEventsRequest>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);
    It should_return_success = () => result.Success.Should().BeTrue();
}