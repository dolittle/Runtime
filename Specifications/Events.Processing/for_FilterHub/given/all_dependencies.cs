// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Runtime.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.for_FilterHub.given
{
    public class all_dependencies
    {
        protected static Mock<ITenants> tenants_mock;
        protected static Mock<IExecutionContextManager> execution_context_manager_mock;
        protected static Mock<FactoryFor<IStreamProcessorHub>> get_stream_processor_hub_mock;
        protected static Mock<FactoryFor<IWriteEventToStream>> get_event_to_stream_writer_mock;
        protected static Mock<IStreamProcessorHub> stream_processor_hub_mock;

        Establish context = () =>
        {
            tenants_mock = new Mock<ITenants>();
            execution_context_manager_mock = new Mock<IExecutionContextManager>();
            stream_processor_hub_mock = new Mock<IStreamProcessorHub>();
            get_stream_processor_hub_mock = new Mock<FactoryFor<IStreamProcessorHub>>();
            get_stream_processor_hub_mock.Setup(_ => _.Invoke()).Returns(stream_processor_hub_mock.Object);
            get_event_to_stream_writer_mock = new Mock<FactoryFor<IWriteEventToStream>>();
            get_event_to_stream_writer_mock.Setup(_ => _.Invoke()).Returns(new in_memory_event_to_stream_writer());
        };
    }
}