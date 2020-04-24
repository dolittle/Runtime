// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.when_catching_up.and_there_is_one_failing_partition.that_should_be_retried.and_succeeds_in_processing
{
    public class all_events : given.all_dependencies
    {
        static StreamProcessorState result;

        Establish context = () =>
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new SucceededProcessingResult()));

        Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, event_processor.Object, stream_processor_state).GetAwaiter().GetResult();

        It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
        It should_not_have_any_failing_partitions = () => result.FailingPartitions.ShouldBeEmpty();
        It should_have_processed_three_events = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
        It should_have_processed_first_event_once = () => event_processor.Verify(_ => _.Process(committed_events[0], failing_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_have_processed_second_event_once = () => event_processor.Verify(_ => _.Process(committed_events[1], failing_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
        It should_have_processed_third_event_once = () => event_processor.Verify(_ => _.Process(committed_events[2], failing_partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}