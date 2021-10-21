// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.given;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.when_setting_position.and_stream_has_only_one_partition
{
    public class and_everything_is_ok : all_dependencies
    {
        const string reason = "some reason";
        static readonly PartitionId partition_id = "paritition id";
        static readonly CommittedEvent first_event = committed_events.single();

        Establish context = () =>
        {
            var event_with_partition = new StreamEvent(first_event, StreamPosition.Start, Guid.NewGuid(), partition_id, true);
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new SuccessfulProcessing()));
            setup_event_stream(event_with_partition);
        };

        Because of = () => start_stream_processor_set_position_after_and_cancel_after(TimeSpan.FromMilliseconds(100), 0, TimeSpan.FromMilliseconds(50)).GetAwaiter().GetResult();

        It should_process_two_event = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
        It should_process_first_event_twice = () => event_processor.Verify(_ => _.Process(first_event, partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));

        It should_have_current_position_equal_one = () => current_stream_processor_state.Position.ShouldEqual(new StreamPosition(1));
        It should_not_have_failing_partition = () => current_stream_processor_state.FailingPartitions.Count.ShouldEqual(0);

    }
}