// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessorHub.given
{
    public class a_scoped_event_processor_hub_configured_with_some_processors : scoped_event_processors
    {
        protected static ScopedEventProcessingHub hub;
        protected static Mock<IExecutionContextManager> mocked_execution_context_manager;

        Establish context = () =>
        {
            mocked_execution_context_manager = mocks.an_execution_context_manager();
            mocked_execution_context_manager.SetupGet(_ => _.Current).Returns(Events.Specs.given.Contexts.get_execution_context());
            hub = new ScopedEventProcessingHub(mocked_execution_context_manager.Object, mocks.a_logger().Object);

            hub.Register(simple_scoped_processor);
            hub.Register(simple_scoped_processor_for_other_tenant);
        };
    }
}