// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler.given;

public class an_event_handler : all_dependencies
{
    protected static EventHandler event_handler;
    protected static Mock<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>> filter_processor;
    protected static Mock<IEventProcessor> event_processor;

    Establish context = () =>
    {
        filter_processor = new Mock<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>>();
        event_processor = new Mock<IEventProcessor>();
        event_handler = new EventHandler(
            stream_processors.Object,
            filter_validation.Object,
            stream_definitions.Object,
            arguments,
            tenant => filter_processor.Object,
            tenant => event_processor.Object,
            cancellation => reverse_call_dispatcher.Object.Accept(new EventHandlerRegistrationResponse(), cancellation),
            (failure, cancellation) => reverse_call_dispatcher.Object.Reject(new EventHandlerRegistrationResponse{Failure = failure.ToProtobuf()}, cancellation),
            logger_factory.CreateLogger<EventHandler>(),
            execution_context,
            cancellation_token
        );
    };
}