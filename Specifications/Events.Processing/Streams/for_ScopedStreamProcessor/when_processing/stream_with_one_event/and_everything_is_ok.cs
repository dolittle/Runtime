// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Streams.for_ScopedStreamProcessor.when_processing.stream_with_one_event;

public class and_everything_is_ok : given.all_dependencies
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
    
    Because of = () => start_stream_processor_and_cancel_after(TimeSpan.FromMilliseconds(500)).GetAwaiter().GetResult();

    It should_process_first_event = () => event_processor.Verify(_ => _.Process(first_event, partition_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<ExecutionContext>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Once());
    It should_have_current_position_equal_one = () => current_stream_processor_state.Position.StreamPosition.ShouldEqual(new StreamPosition(1));
    It should_not_be_failing = () => current_stream_processor_state.IsFailing.ShouldBeFalse();
    It should_try_fetching_next_event = () => events_fetcher.Verify(_ => _.Fetch(1, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
}