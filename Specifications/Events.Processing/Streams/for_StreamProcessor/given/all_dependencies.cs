// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessor.given
{
    public class all_dependencies
    {
        protected static StreamProcessorId stream_processor_id;
        protected static IPerformActionOnAllTenants on_all_tenants;
        protected static IStreamDefinition stream_definition;
        protected static Mock<Func<IEventProcessor>> get_event_processor;
        protected static Mock<FactoryFor<IStreamProcessorStateRepository>> get_stream_processor_state_repository;
        protected static Mock<FactoryFor<IEventFetchers>> get_event_fetchers;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static Mock<ILoggerManager> logger_manager;
        protected static StreamProcessor stream_processor;
        protected static Mock<ITenants> tenants;

        Establish context = () =>
        {
            stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), StreamId.New());
            stream_definition = new StreamDefinition(new FilterDefinition(Guid.NewGuid(), Guid.NewGuid(), false));
            tenants = new Mock<ITenants>();
            logger_manager = new Mock<ILoggerManager>();
            get_event_fetchers = new Mock<FactoryFor<IEventFetchers>>();
            get_event_processor = new Mock<Func<IEventProcessor>>();
            get_stream_processor_state_repository = new Mock<FactoryFor<IStreamProcessorStateRepository>>();
            execution_context_manager = new Mock<IExecutionContextManager>();
            logger_manager
                .Setup(_ => _.CreateLogger<StreamProcessors>())
                .Returns(Mock.Of<ILogger<StreamProcessors>>());
            on_all_tenants = new PerformActionOnAllTenants(tenants.Object, execution_context_manager.Object);
            stream_processor = new StreamProcessor(
                stream_processor_id,
                on_all_tenants,
                stream_definition,
                get_event_processor.Object,
                () => { },
                get_stream_processor_state_repository.Object,
                get_event_fetchers.Object,
                execution_context_manager.Object,
                logger_manager.Object,
                CancellationToken.None);
        };
    }
}