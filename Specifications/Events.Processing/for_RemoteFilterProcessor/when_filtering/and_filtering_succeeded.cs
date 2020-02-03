// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_RemoteFilterProcessor.when_processing
{
    public class and_filtering_succeeded : given.all_dependencies
    {
        const bool is_included = true;
        const int partition = 123;
        static RemoteFilterProcessor remote_processor;
        static IProcessingResult result;

        Establish context = () => remote_processor = new RemoteFilterProcessor(event_processor_id, target_stream_id, Processing.given.a_remote_filter_service(new SucceededFilteringResult(is_included, partition)), event_to_stream_writer_mock.Object, Moq.Mock.Of<ILogger>());

        Because of = async () => result = await remote_processor.Process(an_event);

        It should_succeed_processing = () => result.Succeeded.ShouldEqual(true);
        It should_not_retry_processing = () => result.Retry.ShouldEqual(false);
        It should_be_an_IFilterResult = () => result.ShouldBeAssignableTo<IFilterResult>();
        It should_have_the_correct_is_included_value = () => (result as IFilterResult).IsIncluded.ShouldEqual(is_included);
        It should_have_the_correct_partition_value = () => (result as IFilterResult).Partition.ShouldEqual(partition);
        It should_write_the_event_to_stream = () => event_to_stream_writer_mock.Verify(_ => _.Write(an_event, target_stream_id), Moq.Times.Once());
    }
}