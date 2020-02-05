// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_RemoteFilterProcessor.when_processing
{
    public class and_filtering_needs_retry : given.all_dependencies
    {
        static uint retry_timeout = 123;
        static RemoteFilterProcessor processing_processor;
        static IProcessingResult result;

        Establish context = () => processing_processor = new RemoteFilterProcessor(target_stream_id, Processing.given.a_remote_filter_service(new RetryFilteringResult(retry_timeout)), event_to_stream_writer_mock.Object, Moq.Mock.Of<ILogger>());

        Because of = async () => result = await processing_processor.Process(an_event, partition_id).ConfigureAwait(false);

        It should_not_succeed_processing = () => result.Succeeded.ShouldEqual(false);
        It should_retry_processing = () => result.Retry.ShouldEqual(true);
        It should_be_of_assignable_to_IRetryFilteringResult = () => result.ShouldBeAssignableTo<IRetryFilteringResult>();
        It should_be_of_type_RetryFilteringResult = () => result.ShouldBeOfExactType<RetryFilteringResult>();
        It should_have_the_correct_timeout = () => (result as IRetryFilteringResult).RetryTimeout.ShouldEqual(retry_timeout);
        It should_write_the_event_to_stream = () => event_to_stream_writer_mock.Verify(_ => _.Write(an_event, target_stream_id, partition_id), Moq.Times.Never());
    }
}