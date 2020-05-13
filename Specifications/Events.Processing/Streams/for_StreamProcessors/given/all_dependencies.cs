// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessors.given
{
    public class all_dependencies
    {
        protected static Mock<IPerformActionOnAllTenants> on_all_tenants;
        protected static Mock<ILoggerManager> logger_manager;
        protected static Mock<FactoryFor<IEventFetchers>> get_event_fetchers;
        protected static Mock<FactoryFor<IStreamProcessorStateRepository>> get_stream_processor_state_repository;
        protected static Mock<IExecutionContextManager> execution_context_manager;
        protected static IStreamProcessors stream_processors;

        Establish context = () =>
        {
            on_all_tenants = new Mock<IPerformActionOnAllTenants>();
            logger_manager = new Mock<ILoggerManager>();
            get_event_fetchers = new Mock<FactoryFor<IEventFetchers>>();
            get_stream_processor_state_repository = new Mock<FactoryFor<IStreamProcessorStateRepository>>();
            execution_context_manager = new Mock<IExecutionContextManager>();
            logger_manager.Setup(_ => _.CreateLogger<StreamProcessors>()).Returns(Mock.Of<ILogger<StreamProcessors>>());
            stream_processors = new StreamProcessors(on_all_tenants.Object, get_stream_processor_state_repository.Object, get_event_fetchers.Object, execution_context_manager.Object, logger_manager.Object);
        };
    }
}