// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_HandlerProcessor.when_processing
{
    public class and_handler_needs_retry : given.all_dependencies
    {
        static RemoteEventProcessor handler_processor;
        static IProcessingResult result;

        Establish context = () => handler_processor = new RemoteEventProcessor(event_processor_id, Processing.given.a_handler_service(retry_handling_result));

        Because of = async () => result = await handler_processor.Process(an_event);

        It should_not_succeed_processing = () => result.Succeeded.ShouldEqual(false);
        It should_retry_processing = () => result.Retry.ShouldEqual(true);
        It should_be_of_type_RetryProcessingResult = () => result.ShouldBeOfExactType<RetryProcessingResult>();
        It should_have_the_correct_timeout = () => (result as RetryProcessingResult).RetryTimeout.ShouldEqual(retry_timeout);
    }
}