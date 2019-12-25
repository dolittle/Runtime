// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Dolittle.Runtime.Events.Processing;
using Machine.Specifications;
using Moq;
using specs = Dolittle.Runtime.Events.Specs.given;

namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given
{
    public class a_scoped_event_processor_hub
    {
        protected static ScopedEventProcessingHub hub;
        protected static Mock<IExecutionContextManager> mocked_execution_context_manager;

        Establish context = () =>
        {
            mocked_execution_context_manager = mocks.an_execution_context_manager();
            var execution_context = specs.Contexts.get_execution_context();
            mocked_execution_context_manager.SetupGet(m => m.Current).Returns(execution_context);
            hub = new ScopedEventProcessingHub(mocked_execution_context_manager.Object, mocks.a_logger().Object);
        };
    }
}