// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_started.and_aggregate_events_are_committed;

public class and_updating_states_fails : given.all_dependencies
{
    static Task<Try> result;
    Establish context = () =>
    {
        event_waiter.Setup(_ => _.WaitForEvent(ScopeId.Default, StreamId.EventLog, Moq.It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        state_updater.Setup(_ => _.TryUpdateAll(execution_context, Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(Try.Failed(new Exception())));
    };

    Because of = () => result = embedding_processor.Start(cancellation_token);

    It should_return_a_failure = () => result.Result.Success.ShouldBeFalse();
    It should_update_embedding_states = () => state_updater.Verify(_ => _.TryUpdateAll(execution_context, Moq.It.IsAny<CancellationToken>()), Times.Exactly(1));
    It should_wait_for_aggregate_events = () => event_waiter.Verify(_ => _.WaitForEvent(ScopeId.Default, StreamId.EventLog, Moq.It.IsAny<CancellationToken>()), Times.Exactly(0));
}