// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.given;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.when_setting_position.a_stream_that_has_events_in_two_partitions
{
    public class and_everything_is_ok : all_dependencies
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
                .Returns(Task.FromResult<IProcessingResult>(new SuccessfulProcessing()));
            setup_event_stream(first_event, second_event);
            events_fetcher
                .Setup(_ => _.FetchInPartition(first_partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Try<StreamEvent>>(first_event));
        };

        Because of = () => start_stream_processor_set_position_after_and_cancel_after(TimeSpan.FromMilliseconds(100), 0, TimeSpan.FromMilliseconds(50)).GetAwaiter().GetResult();
        
        It should_process_four_times = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(4));
        It should_process_first_event_twice = () => event_processor.Verify(_ => _.Process(first_event.Event, first_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
        It should_process_second_event_twice = () => event_processor.Verify(_ => _.Process(second_event.Event, second_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
        It should_not_retry_processing = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<string>(), Moq.It.IsAny<uint>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);

        It should_have_persisted_the_correct_position = () => current_stream_processor_state.Position.ShouldEqual(new StreamPosition(2));
        It should_not_have_failing_partition = () => current_stream_processor_state.FailingPartitions.Count.ShouldEqual(0);
    }
}