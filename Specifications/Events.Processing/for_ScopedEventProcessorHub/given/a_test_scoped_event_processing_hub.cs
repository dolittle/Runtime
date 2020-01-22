// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Execution;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using Moq;
using processors = Dolittle.Runtime.Events.Processing.Specs.given;
using specs = Dolittle.Runtime.Events.Specs.given;

namespace Dolittle.Runtime.Events.Processing.Specs.for_ScopedEventProcessorHub.given
{
    public class a_test_scoped_event_processing_hub
    {
        protected static TestScopedEventProcessingHub hub;
        protected static Mock<IExecutionContextManager> mocked_execution_context_manager;
        protected static List<CommittedEventStream> commits;

        protected static int execution_context_sets = 0;
        protected static ExecutionContext current_execution_context;

        Establish context = () =>
        {
            var execution_context = specs.Contexts.get_execution_context();
            mocked_execution_context_manager = mocks.an_execution_context_manager();
            mocked_execution_context_manager.SetupGet(m => m.Current)
                .Returns(execution_context);
            hub = new TestScopedEventProcessingHub(mocked_execution_context_manager.Object, mocks.a_logger().Object);
            commits = processors.committed_event_streams();
        };
    }
}