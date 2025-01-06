// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.for_ScopedStreamProcessor.when_processing;

public class and_event_processing_failed_at_second_event : given.all_dependencies
{
    const string reason = "some reason";
    static readonly PartitionId partition_id = "partition id";
    static readonly CommittedEvent first_event = committed_events.single();
    static readonly CommittedEvent second_event = committed_events.single();

    Establish context = () =>
    {
        event_processor
            .Setup(_ => _.Process(first_event, partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(SuccessfulProcessing.Instance));
        event_processor
            .Setup(_ => _.Process(second_event, partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(),Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(reason)));
        setup_event_stream(
            new StreamEvent(first_event, 0, Guid.NewGuid(), partition_id, true),
            new StreamEvent(second_event, 1, Guid.NewGuid(), partition_id, false));
    };


    Because of = () => start_stream_processor_and_cancel_after(TimeSpan.FromMilliseconds(250)).GetAwaiter().GetResult();

    It should_process_two_events = () => event_processor.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(),Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
    It should_process_first_event = () => event_processor.Verify(_ => _.Process(first_event, partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(),Moq.It.IsAny<CancellationToken>()), Moq.Times.Once());
    It should_process_second_event = () => event_processor.Verify(_ => _.Process(second_event, partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(),Moq.It.IsAny<CancellationToken>()), Moq.Times.Once());
    It should_have_current_position_equal_one = () => current_stream_processor_state.Position.StreamPosition.ShouldEqual(new StreamPosition(1));
    It should_be_failing = () => current_stream_processor_state.IsFailing.ShouldBeTrue();
    It should_have_the_correct_retry_time = () => current_stream_processor_state.RetryTime.ShouldEqual(DateTimeOffset.MaxValue);
    It should_have_the_correct_reason = () => current_stream_processor_state.FailureReason.ShouldEqual(reason);
    It should_have_one_processing_attempt = () => current_stream_processor_state.ProcessingAttempts.ShouldEqual(1u);
}