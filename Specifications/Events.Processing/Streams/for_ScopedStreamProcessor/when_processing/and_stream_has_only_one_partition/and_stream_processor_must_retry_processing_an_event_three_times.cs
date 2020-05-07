// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_ScopedStreamProcessor.when_processing.and_stream_has_only_one_partition
{
    [Ignore("Not implemented")]
    public class and_stream_processor_must_retry_processing_an_event_three_times : given.all_dependencies
    {
        const string failure_reason = "some reason";
        const string retry_reason = "retry reason";
        static readonly PartitionId partition_id = Guid.NewGuid();
        static readonly CommittedEvent first_event = committed_events.single();

        Establish context = () =>
        {
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(retry_reason, true, TimeSpan.Zero)));
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<string>(), 0, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(retry_reason, true, TimeSpan.Zero)));
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<string>(), 1, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(retry_reason, true, TimeSpan.Zero)));
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<string>(), 2, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(failure_reason)));
            events_fetcher
                .Setup(_ => _.Fetch(0, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new StreamEvent(first_event, 0, Guid.NewGuid(), partition_id)));
            events_fetcher
                .Setup(_ => _.Fetch(1, Moq.It.IsAny<CancellationToken>()))
                .Throws(new Exception());
        };

        Because of = () => stream_processor.Start().GetAwaiter().GetResult();

        It should_process_first_event_normally_once = () => event_processor.Verify(_ => _.Process(first_event, partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_retry_processing_first_event_first_time_with_correct_reason = () => event_processor.Verify(_ => _.Process(first_event, partition_id, retry_reason, 0, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_retry_processing_first_event_second_time_with_correct_reason = () => event_processor.Verify(_ => _.Process(first_event, partition_id, retry_reason, 1, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_retry_processing_first_event_third_time_with_correct_reason = () => event_processor.Verify(_ => _.Process(first_event, partition_id, retry_reason, 2, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

        // It should_have_current_position_equal_one = () => stream_processor.CurrentState.Position.ShouldEqual(new StreamPosition(1));
        // It should_have_one_failing_partition = () => stream_processor.CurrentState.FailingPartitions.Count.ShouldEqual(1);
        // It should_have_the_correct_failing_partition = () => stream_processor.CurrentState.FailingPartitions.ContainsKey(partition_id).ShouldBeTrue();
        // It should_have_the_correct_position_on_the_failing_partition = () => stream_processor.CurrentState.FailingPartitions[partition_id].Position.ShouldEqual(new StreamPosition(0));

        // It should_have_the_correct_reason_on_the_failing_partition = () => stream_processor.CurrentState.FailingPartitions[partition_id].Reason.ShouldEqual(failure_reason);
        // It should_have_the_correct_retry_time_on_the_failing_partition = () => stream_processor.CurrentState.FailingPartitions[partition_id].RetryTime.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
    }
}