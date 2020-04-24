// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Globalization;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Security;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_EventProcessor.when_processing
{
    public class an_event : given.all_dependencies
    {
        static ExecutionContext current_execution_context;
        static EventProcessor event_processor;
        static CommittedEvent @event;
        static PartitionId partition;
        static EventHandlerRuntimeToClientRequest request;

        Establish context = () =>
        {
            current_execution_context = new ExecutionContext(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "",
                Guid.NewGuid(),
                Claims.Empty,
                CultureInfo.InvariantCulture);
            execution_context_manager.SetupGet(_ => _.Current).Returns(current_execution_context);
            event_processor = new EventProcessor(event_processor_id, call_dispatcher.Object, execution_context_manager.Object, Moq.Mock.Of<ILogger>());
            @event = new CommittedEvent(
                0,
                DateTimeOffset.Now,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                new Artifact(Guid.NewGuid(), 0),
                true,
                "");
            partition = Guid.NewGuid();

            request = new EventHandlerRuntimeToClientRequest
            {
                Event = @event.ToProtobuf(),
                Partition = partition.ToProtobuf(),
                ExecutionContext = current_execution_context.ToByteString()
            };
        };

        Because of = () => event_processor.Process(@event, partition, default).GetAwaiter().GetResult();

        It should_call_the_call_dispatcher_with_the_correct_request = () => call_dispatcher.Verify(_ => _.Call(
            request,
            Moq.It.IsAny<Action<EventHandlerClientToRuntimeResponse>>()));
    }
}