// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.when_processing.and_stream_has_only_one_partition
{
    public class and_event_processing_failed_at_second_event : given.all_dependencies
    {
        const string reason = "some reason";
        static readonly PartitionId partition_id = "a partition";
        static readonly CommittedEvent first_event = committed_events.single();
        static readonly CommittedEvent second_event = committed_events.single();

        Establish context = () =>
        {
            event_processor
                .Setup(_ => _.Process(first_event, partition_id, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new SuccessfulProcessing()));
            event_processor
                .Setup(_ => _.Process(second_event, partition_id, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(reason)));
            events_fetcher
                .Setup(_ => _.Fetch(0, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<StreamEvent>>(new StreamEvent(first_event, 0, Guid.NewGuid(), partition_id, true)));
            events_fetcher
                .Setup(_ => _.Fetch(1, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<StreamEvent>>(new StreamEvent(second_event, 1, Guid.NewGuid(), partition_id, true)));
            events_fetcher
                .Setup(_ => _.Fetch(2, Moq.It.IsAny<CancellationToken>()))
                .Throws(new Exception());
        };

        Because of = () => stream_processor.Start(CancellationToken.None).GetAwaiter().GetResult();

        It should_process_two_events = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
        It should_process_first_event = () => event_processor.Verify(_ => _.Process(first_event, partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once());
        It should_process_second_event = () => event_processor.Verify(_ => _.Process(second_event, partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once());

        It should_have_current_position_equal_1 = () => current_state.Position.ShouldEqual(new StreamPosition(2));
        It should_have_one_failing_partition = () => current_state.FailingPartitions.Count.ShouldEqual(1);
        It should_have_the_correct_failing_partition = () => current_state.FailingPartitions.ContainsKey(partition_id).ShouldBeTrue();
        It should_have_the_correct_position_on_the_failing_partition = () => current_state.FailingPartitions[partition_id].Position.ShouldEqual(new StreamPosition(1));
        It should_have_the_correct_reason_on_the_failing_partition = () => current_state.FailingPartitions[partition_id].Reason.ShouldEqual(reason);
        It should_have_the_correct_retry_time_on_the_failing_partition = () => current_state.FailingPartitions[partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);

        static StreamProcessorState current_state => stream_processor_state_repository.TryGetFor(stream_processor_id, CancellationToken.None).GetAwaiter().GetResult().Result as StreamProcessorState;
    }
}