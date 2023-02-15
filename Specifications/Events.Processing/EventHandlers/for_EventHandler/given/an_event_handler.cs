// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Processing.EventHandlers.Actors;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using Proto;

namespace Dolittle.Runtime.Events.Processing.EventHandlers.for_EventHandler.given;

public class an_event_handler : all_dependencies
{
    protected static ActorEventHandler event_handler;
    protected static Mock<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>> filter_processor;
    protected static Mock<IEventProcessor> event_processor;

    Establish context = () =>
    {
        filter_processor = new Mock<IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition>>();
        event_processor = new Mock<IEventProcessor>();
        event_handler = new ActorEventHandler(
            stream_processors.Object,
            stream_definitions.Object,
            arguments,
            tenant => event_processor.Object,
            cancellation => reverse_call_dispatcher.Object.Accept(new EventHandlerRegistrationResponse(), cancellation),
            (failure, cancellation) => reverse_call_dispatcher.Object.Reject(new EventHandlerRegistrationResponse{Failure = failure.ToProtobuf()}, cancellation),
            metrics_collector.Object,
            logger_factory.CreateLogger<ActorEventHandler>(),
            execution_context,
            actor_system,
            tenants,
            create_processor_props,
            cancellation_token
        );
    };
}