// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_started.and_aggregate_events_are_committed
{
    public class and_updating_states_fails : given.all_dependencies
    {
        static CancellationToken cancellation_token;
        static Task<Try> result;
        Establish context = () =>
        {
            cancellation_token = CancellationToken.None;
            event_waiter
                .Setup(_ => _.WaitForEvent(embedding.Value, cancellation_token))
                .Returns(Task.CompletedTask);
            state_updater.Setup(_ => _.TryUpdateAllFor(embedding, cancellation_token)).Returns(Task.FromResult(Try.Failed()));
        };

        Because of = () => result = embedding_processor.Start(cancellation_token);

        It should_not_be_running = () => result.Status.ShouldEqual(TaskStatus.Canceled);
        It should_return_a_failure = () => result.Result.Success.ShouldBeFalse();
        It should_update_embedding_states = () => state_updater.Verify(_ => _.TryUpdateAllFor(embedding, CancellationToken.None), Times.Exactly(1));
        It should_wait_for_aggregate_events = () => event_waiter.Verify(_ => _.WaitForEvent(embedding.Value, CancellationToken.None), Times.Exactly(0));
    }
}