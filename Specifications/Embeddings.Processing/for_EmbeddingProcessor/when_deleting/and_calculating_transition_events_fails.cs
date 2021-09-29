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

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_deleting
{
    public class and_calculating_transition_events_fails : given.all_dependencies_and_a_key
    {
        static Task task;
        static Exception exception;

        Establish context = () =>
        {
            exception = new Exception();
            task = embedding_processor.Start(cancellation_token);
            embedding_store
                .Setup(_ => _.TryGet(embedding, key, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingCurrentState>.Succeeded(current_state)));
            transition_calculator
                .Setup(_ => _.TryDelete(current_state, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<UncommittedAggregateEvents>.Failed(exception)));
        };

        static Try result;

        Because of = () => result = embedding_processor.Delete(key, cancellation_token).GetAwaiter().GetResult();

        It should_still_be_running = () => task.Status.ShouldEqual(TaskStatus.WaitingForActivation);
        It should_fetch_the_current_state = () => embedding_store.Verify(_ => _.TryGet(embedding, key, Moq.It.IsAny<CancellationToken>()));
        It should_return_the_failure = () => result.Exception.ShouldEqual(exception);
    }
}
