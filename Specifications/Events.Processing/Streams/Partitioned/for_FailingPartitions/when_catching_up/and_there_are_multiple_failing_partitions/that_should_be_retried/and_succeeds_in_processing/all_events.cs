// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up.and_there_are_multiple_failing_partitions.that_should_be_retried.and_succeeds_in_processing;

public class all_events : given.all_dependencies
{
    static StreamProcessorState result;

    Establish context = () =>
    {
        event_processor
            .Setup(_ => _.Process(
                Moq.It.IsAny<CommittedEvent>(),
                Moq.It.IsAny<PartitionId>(),
                Moq.It.IsAny<StreamPosition>(),
                Moq.It.IsAny<string>(),
                Moq.It.IsAny<uint>(),
                Moq.It.IsAny<ExecutionContext>(),
                Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(SuccessfulProcessing.Instance));
        
        stream_processor_state_repository.Persist(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult();
    };

    Because of = () => result = failing_partitions.CatchupFor(stream_processor_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult() as StreamProcessorState;

    It should_return_the_same_stream_position = () => result.Position.ShouldEqual(stream_processor_state.Position);
    It should_not_have_any_failing_partitions = () => result.FailingPartitions.ShouldBeEmpty();

    It should_have_first_failing_partition_process_two_events = () => event_processor.Verify(
        _ => _.Process(
            Moq.It.IsAny<CommittedEvent>(),
            first_failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));

    It should_have_second_failing_partition_process_one_event = () => event_processor.Verify(
        _ => _.Process(
            Moq.It.IsAny<CommittedEvent>(),
            second_failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(1));

    It should_have_retried_processeing_three_times = () => event_processor.Verify(
        _ => _.Process(
            Moq.It.IsAny<CommittedEvent>(),
            Moq.It.IsAny<PartitionId>(),
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));

    It should_have_first_failing_partition_process_first_event_once = () => event_processor.Verify(
        _ => _.Process(
            eventStream[0].Event,
            first_failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

    It should_have_second_failing_partition_process_second_event_once = () => event_processor.Verify(
        _ => _.Process(
            eventStream[1].Event,
            second_failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

    It should_have_first_failing_process_process_third_event_once = () => event_processor.Verify(
        _ => _.Process(
            eventStream[2].Event,
            first_failing_partition_id,
            Moq.It.IsAny<StreamPosition>(),
            Moq.It.IsAny<string>(),
            Moq.It.IsAny<uint>(),
            Moq.It.IsAny<ExecutionContext>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
}