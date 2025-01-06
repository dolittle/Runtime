// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventProcessor.when_retrying_processing;

public class an_event : given.all_dependencies
{
    static Execution.ExecutionContext current_execution_context;
    static EventProcessor event_processor;
    static CommittedEvent @event;
    static PartitionId partition;

    Establish context = () =>
    {
        current_execution_context = execution_contexts.create();
        event_processor = new EventProcessor(scope, event_processor_id, dispatcher.Object, Moq.Mock.Of<ILogger>());
        dispatcher
            .Setup(_ => _.Call(Moq.It.IsAny<Contracts.HandleEventRequest>(), Moq.It.IsAny<ExecutionContext>(), CancellationToken.None))
            .Returns(Task.FromResult(new Contracts.EventHandlerResponse()));
        @event = new CommittedEvent(
            0,
            DateTimeOffset.Now,
            "some event source",
            execution_contexts.create(),
            new Artifact(Guid.NewGuid(), 0),
            true,
            "");
        partition = "a partition";
    };

    Because of = () => event_processor.Process(@event, partition, StreamPosition.Start, execution_context, default).GetAwaiter().GetResult();

    It should_call_the_dispatcher_once = () => dispatcher.Verify(_ => _.Call(Moq.It.IsAny<Contracts.HandleEventRequest>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
}