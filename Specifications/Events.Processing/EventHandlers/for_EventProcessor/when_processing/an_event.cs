// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventProcessor.when_processing
{
    [Ignore("Not implemented")]
    public class an_event : given.all_dependencies
    {
        static ExecutionContext current_execution_context;
        static EventProcessor event_processor;
        static CommittedEvent @event;
        static PartitionId partition;

        Establish context = () =>
        {
            current_execution_context = execution_contexts.create();
            execution_context_manager.SetupGet(_ => _.Current).Returns(current_execution_context);
            event_processor = new EventProcessor(scope, event_processor_id, dispatcher.Object, Moq.Mock.Of<ILogger>());
            @event = new CommittedEvent(
                0,
                DateTimeOffset.Now,
                Guid.NewGuid(),
                execution_contexts.create(),
                new Artifact(Guid.NewGuid(), 0),
                true,
                "");
            partition = Guid.NewGuid();
        };

        Because of = () => event_processor.Process(@event, partition, default).GetAwaiter().GetResult();
    }
}