// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.for_StreamProcessorHub.given
{
    public class all_dependencies
    {
        protected static Mock<IExecutionContextManager> execution_context_manager_mock;
        protected static Mock<IFetchEventsFromStreams> next_event_fetcher_mock;
        protected static IStreamProcessorStateRepository stream_processor_state_repository;

        Establish context = () =>
        {
            execution_context_manager_mock = new Mock<IExecutionContextManager>();
            next_event_fetcher_mock = Processing.given.a_next_event_fetcher_mock();
            stream_processor_state_repository = new in_memory_stream_processor_state_repository();
        };
    }
}