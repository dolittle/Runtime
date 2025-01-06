// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.given;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.when_setting_position.and_stream_has_only_one_partition;

public class and_setting_position_to_failing_event : all_dependencies
{
    const string failure_reason = "some reason";
    const string retry_reason = "retry reason";
    static readonly PartitionId partition_id = "parTITION id";
    static readonly CommittedEvent first_event = committed_events.single();

    Establish context = () =>
    {
        event_processor
            .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(failure_reason)));
        setup_event_stream(new StreamEvent(first_event, 0, Guid.NewGuid(), partition_id, true));
    };

    Because of = () => start_stream_processor_set_position_after_and_cancel_after(TimeSpan.FromMilliseconds(100), ProcessingPosition.Initial, action_to_perform_before_reprocessing.Object, TimeSpan.FromMilliseconds(50)).GetAwaiter().GetResult();

    It should_process_first_event_twice = () => event_processor.Verify(_ => _.Process(first_event, partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
    It should_have_current_position_equal_one = () => current_stream_processor_state.Position.StreamPosition.ShouldEqual(new StreamPosition(1));
    It should_have_one_failing_partition = () => current_stream_processor_state.FailingPartitions.Count.ShouldEqual(1);
    It should_have_the_correct_failing_partition = () => current_stream_processor_state.FailingPartitions.ContainsKey(partition_id).ShouldBeTrue();
    It should_have_the_correct_position_on_the_failing_partition = () => current_stream_processor_state.FailingPartitions[partition_id].Position.StreamPosition.ShouldEqual(new StreamPosition(0));
    It should_have_the_correct_reason_on_the_failing_partition = () => current_stream_processor_state.FailingPartitions[partition_id].Reason.ShouldEqual(failure_reason);
    It should_have_the_correct_retry_time_on_the_failing_partition = () => current_stream_processor_state.FailingPartitions[partition_id].RetryTime.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
    It should_perform_the_action = () => action_to_perform_before_reprocessing.Verify(_ => _(tenant_id, Moq.It.IsAny<CancellationToken>()), Times.Once);
}