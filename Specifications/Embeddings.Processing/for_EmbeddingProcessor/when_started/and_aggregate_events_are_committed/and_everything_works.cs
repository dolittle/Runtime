// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_started.and_aggregate_events_are_committed
{
    public class and_everything_works : given.all_dependencies
    {
        static int called_wait_num;
        static Task<Try> result;
        Establish context = () =>
        {
            called_wait_num = 0;
            event_waiter
                .Setup(_ => _.WaitForEvent(ScopeId.Default, StreamId.EventLog, Moq.It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    return called_wait_num++ switch
                    {
                        0 => Task.CompletedTask,
                        _ => Task.Delay(Timeout.Infinite)
                    };
                });
            state_updater.Setup(_ => _.TryUpdateAll(Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(Try.Succeeded()));
        };

        Because of = () =>
        {
            result = embedding_processor.Start(cancellation_token);
            Thread.Sleep(100);
        };

        It should_be_running = () => result.Status.ShouldEqual(TaskStatus.WaitingForActivation);
        It should_update_embedding_states = () => state_updater.Verify(_ => _.TryUpdateAll(Moq.It.IsAny<CancellationToken>()), Times.Exactly(2));
        It should_wait_for_aggregate_events = () => event_waiter.Verify(_ => _.WaitForEvent(ScopeId.Default, StreamId.EventLog, Moq.It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}