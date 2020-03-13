// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Services;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.for_EventProcessor.given
{
    public class all_dependencies
    {
        protected static EventProcessorId event_processor_id;
        protected static Mock<IReverseCallDispatcher<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest>> call_dispatcher;
        protected static Mock<IExecutionContextManager> execution_context_manager;

        Establish context = () =>
        {
            event_processor_id = Guid.NewGuid();
            call_dispatcher = new Mock<IReverseCallDispatcher<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest>>();
            execution_context_manager = new Mock<IExecutionContextManager>();
        };
    }
}