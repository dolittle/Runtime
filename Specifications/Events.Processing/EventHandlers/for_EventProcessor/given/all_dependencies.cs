// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventProcessor.given;

public class all_dependencies
{
    protected static ScopeId scope;
    protected static EventProcessorId event_processor_id;
    protected static Mock<IReverseCallDispatcher<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>> dispatcher;
    protected static ExecutionContext execution_context;

    Establish context = () =>
    {
        execution_context = execution_contexts.create();
        scope = Guid.NewGuid();
        event_processor_id = Guid.NewGuid();
        dispatcher = new Mock<IReverseCallDispatcher<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse>>();
    };
}