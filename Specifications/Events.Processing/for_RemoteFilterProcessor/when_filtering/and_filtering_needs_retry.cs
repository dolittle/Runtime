// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_RemoteFilterProcessor.when_processing
{
    public class and_filtering_needs_retry : given.all_dependencies
    {
        static int retry_timeout = 123;
        static RemoteFilterProcessor processing_processor;
        static IProcessingResult result;

        Establish context = () => processing_processor = new RemoteFilterProcessor(event_processor_id, target_stream_id, Processing.given.a_remote_filter_service(new RetryFilteringResult(retry_timeout)), new in_memory_event_to_stream_writer());

        Because of = async () => result = await processing_processor.Process(an_event);

        It should_not_succeed_processing = () => result.Succeeded.ShouldEqual(false);
        It should_retry_processing = () => result.Retry.ShouldEqual(true);
        It should_be_of_type_RetryFilteringResult = () => result.ShouldBeOfExactType<RetryFilteringResult>();
        It should_have_the_correct_timeout = () => (result as RetryFilteringResult).RetryTimeout.ShouldEqual(retry_timeout);
    }
}