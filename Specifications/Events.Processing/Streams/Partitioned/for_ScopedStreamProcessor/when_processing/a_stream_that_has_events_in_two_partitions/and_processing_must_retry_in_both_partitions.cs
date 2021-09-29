// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.when_processing.a_stream_that_has_events_in_two_partitions
{
    public class and_processing_must_retry_in_both_partitions : given.all_dependencies
    {
        const string reason = "some reason";
        static PartitionId first_partition_id;
        static PartitionId second_partition_id;
        static StreamEvent first_event;
        static StreamEvent second_event;

        Establish context = () =>
        {
            first_partition_id = Guid.NewGuid();
            second_partition_id = Guid.NewGuid();
            first_event = new StreamEvent(committed_events.single(), StreamPosition.Start, Guid.NewGuid(), first_partition_id, true);
            second_event = new StreamEvent(committed_events.single(), 1u, Guid.NewGuid(), second_partition_id, true);
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(reason, true, TimeSpan.Zero)));
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(reason)));
            events_fetcher
                .Setup(_ => _.Fetch(0, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<StreamEvent>>(first_event));
            events_fetcher
                .Setup(_ => _.Fetch(1, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<StreamEvent>>(second_event));
            events_fetcher
                .Setup(_ => _.FetchInPartition(first_partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<StreamEvent>>(first_event));
            events_fetcher
                .Setup(_ => _.FetchInPartition(second_partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<StreamEvent>>(second_event));
            events_fetcher
                .Setup(_ => _.Fetch(2, Moq.It.IsAny<CancellationToken>()))
                .Throws(new Exception());
        };

        Because of = () => stream_processor.Start(CancellationToken.None).GetAwaiter().GetResult();

        It should_process_one_event_in_first_partition = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), first_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_process_one_event_in_second_partition = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), second_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_process_first_event = () => event_processor.Verify(_ => _.Process(first_event.Event, first_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_process_second_event = () => event_processor.Verify(_ => _.Process(second_event.Event, second_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_retry_processing_twice = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
        It should_retry_processing_event_in_first_partition_once = () => event_processor.Verify(_ => _.Process(first_event.Event, first_partition_id, Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_retry_processing_event_in_second_partition_once = () => event_processor.Verify(_ => _.Process(second_event.Event, second_partition_id, Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

        It should_have_persisted_the_correct_position = () => current_state.Position.ShouldEqual(new StreamPosition(2));
        It should_have_persisted_state_with_two_failing_partitions = () => current_state.FailingPartitions.Count.ShouldEqual(2);
        It should_have_persisted_state_with_correct_first_failing_partition = () => current_state.FailingPartitions.ContainsKey(first_partition_id).ShouldBeTrue();
        It should_have_persisted_state_with_correct_second_failing_partition = () => current_state.FailingPartitions.ContainsKey(second_partition_id).ShouldBeTrue();
        It should_have_persisted_correct_position_on_the_first_failing_partition = () => current_state.FailingPartitions[first_partition_id].Position.ShouldEqual(new StreamPosition(0));
        It should_have_persisted_correct_position_on_the_second_failing_partition = () => current_state.FailingPartitions[second_partition_id].Position.ShouldEqual(new StreamPosition(1));
        It should_have_persisted_correct_retry_time_on_the_first_failing_partition = () => current_state.FailingPartitions[first_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
        It should_have_persisted_correct_retry_time_on_the_second_failing_partition = () => current_state.FailingPartitions[second_partition_id].RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
        It should_have_persisted_correct_reason_on_the_first_failing_partition = () => current_state.FailingPartitions[first_partition_id].Reason.ShouldEqual(reason);
        It should_have_persisted_correct_reason_on_the_first_second_partition = () => current_state.FailingPartitions[second_partition_id].Reason.ShouldEqual(reason);

        static StreamProcessorState current_state => stream_processor_state_repository.TryGetFor(stream_processor_id, CancellationToken.None).GetAwaiter().GetResult().Result as StreamProcessorState;
    }
}