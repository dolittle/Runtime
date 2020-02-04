// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_RemoteEventProcessor.when_processing
{
    public class and_processing_needs_retry : given.all_dependencies
    {
        static RemoteEventProcessor processing_processor;
        static IProcessingResult result;

        Establish context = () => processing_processor = new RemoteEventProcessor(event_processor_id, Processing.given.a_remote_processor_service(retry_handling_result), Moq.Mock.Of<ILogger>());

        Because of = async () => result = await processing_processor.Process(an_event);

        It should_not_succeed_processing = () => result.Succeeded.ShouldEqual(false);
        It should_retry_processing = () => result.Retry.ShouldEqual(true);
        It should_be_of_type_RetryProcessingResult = () => result.ShouldBeOfExactType<RetryProcessingResult>();
        It should_have_the_correct_timeout = () => (result as RetryProcessingResult).RetryTimeout.ShouldEqual(retry_timeout);
    }
}