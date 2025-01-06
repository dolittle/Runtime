// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams.for_ScopedStreamProcessor.given;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.for_ScopedStreamProcessor.when_setting_position;

public class and_everything_is_ok : all_dependencies
{
    static readonly PartitionId partition_id = "f6b7a2a4-9d1d-4d9a-8ad5-dbde3bb08ade";
    static readonly CommittedEvent first_event = committed_events.single();

    Establish context = () =>
    {
        var event_with_partition = new StreamEvent(first_event, StreamPosition.Start, Guid.NewGuid(), partition_id, false);
        event_processor
            .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IProcessingResult>(SuccessfulProcessing.Instance));
        setup_event_stream(event_with_partition);
    };

    Because of = () => start_stream_processor_set_position_after_and_cancel_after(TimeSpan.FromMilliseconds(100),ProcessingPosition.Initial, action_to_perform_before_reprocessing.Object, TimeSpan.FromMilliseconds(50)).GetAwaiter().GetResult();
        
    It should_process_event_twice = () => event_processor.Verify(_ => _.Process(first_event, partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
    It should_fetch_event_twice = () => events_fetcher.Verify(_ => _.Fetch(0, Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));
    It should_have_current_position_equal_one = () => current_stream_processor_state.Position.StreamPosition.ShouldEqual(new StreamPosition(1));
    It should_not_be_failing = () => current_stream_processor_state.IsFailing.ShouldBeFalse();
    It should_try_fetching_next_event = () => events_fetcher.Verify(_ => _.Fetch(1, Moq.It.IsAny<CancellationToken>()), Moq.Times.AtLeastOnce);
    It should_perform_the_action = () => action_to_perform_before_reprocessing.Verify(_ => _(tenant_id, Moq.It.IsAny<CancellationToken>()), Times.Once);
}