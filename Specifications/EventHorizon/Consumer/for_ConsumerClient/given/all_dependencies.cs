// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Services.Clients;
using Machine.Specifications;
using Moq;
using grpc = contracts::Dolittle.Runtime.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_ConsumerClient.given
{
    public class all_dependencies
    {
        protected static EventHorizon event_horizon;
        protected static MicroserviceAddress microservice_address;
        protected static Mock<IEventFromEventHorizonValidator> event_from_event_horizon_validator;
        protected static Mock<IClientManager> client_manager;
        protected static Mock<grpc.Consumer.ConsumerClient> grpc_consumer_client;
        protected static IExecutionContextManager execution_context_manager;
        protected static Mock<IStreamProcessors> stream_processors;
        protected static Mock<FactoryFor<IStreamProcessors>> get_stream_processors;
        protected static Mock<IStreamProcessorStateRepository> stream_processor_states;
        protected static Mock<FactoryFor<IStreamProcessorStateRepository>> get_stream_processor_states;
        protected static Mock<IWriteEventHorizonEvents> event_horizon_events_writer;
        protected static Mock<FactoryFor<IWriteEventHorizonEvents>> get_event_horizon_events_writer;
        protected static ConsumerClient consumer_client;

        Establish context = () =>
        {
            event_horizon = new EventHorizon(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            microservice_address = new MicroserviceAddress { Host = "host", Port = 10000 };
            event_from_event_horizon_validator = new Mock<IEventFromEventHorizonValidator>();
            client_manager = new Mock<IClientManager>();
            grpc_consumer_client = new Mock<grpc.Consumer.ConsumerClient>();
            client_manager.Setup(_ => _.Get(typeof(grpc.Consumer.ConsumerClient), Moq.It.IsAny<string>(), Moq.It.IsAny<int>())).Returns(grpc_consumer_client.Object);
            client_manager.Setup(_ => _.Get<grpc.Consumer.ConsumerClient>(Moq.It.IsAny<string>(), Moq.It.IsAny<int>())).Returns(grpc_consumer_client.Object);
            execution_context_manager = new ExecutionContextManager(Mock.Of<ILogger>());
            stream_processors = new Mock<IStreamProcessors>();
            get_stream_processors = new Mock<FactoryFor<IStreamProcessors>>();
            get_stream_processors.Setup(_ => _.Invoke()).Returns(stream_processors.Object);
            stream_processor_states = new Mock<IStreamProcessorStateRepository>();
            get_stream_processor_states = new Mock<FactoryFor<IStreamProcessorStateRepository>>();
            get_stream_processor_states.Setup(_ => _.Invoke()).Returns(stream_processor_states.Object);
            event_horizon_events_writer = new Mock<IWriteEventHorizonEvents>();
            get_event_horizon_events_writer = new Mock<FactoryFor<IWriteEventHorizonEvents>>();
            get_event_horizon_events_writer.Setup(_ => _.Invoke()).Returns(event_horizon_events_writer.Object);

            consumer_client = new ConsumerClient(
                event_from_event_horizon_validator.Object,
                client_manager.Object,
                execution_context_manager,
                get_stream_processors.Object,
                get_stream_processor_states.Object,
                get_event_horizon_events_writer.Object,
                Mock.Of<ILogger>());
        };
    }
}