// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingProcessor.when_starting;

public class once : given.all_dependencies
{
    static Task<Try> result;

    Because of = () => result = embedding_processor.Start(cancellation_token);

    It should_be_running = () => result.Status.ShouldEqual(TaskStatus.WaitingForActivation);
    It should_update_embedding_states = () => state_updater.Verify(_ => _.TryUpdateAll(Moq.It.IsAny<CancellationToken>()));
    It should_wait_for_aggregate_events = () => event_waiter.Verify(_ => _.WaitForEvent(ScopeId.Default, StreamId.EventLog, Moq.It.IsAny<CancellationToken>()));
}