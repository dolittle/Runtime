// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessor.when_stream_processor_is_processing.and_stream_has_only_one_partition
{
    public class and_stream_processor_must_retry_processing_an_event_three_times : given.all_dependencies
    {
        const string failure_reason = "some reason";
        static readonly PartitionId partition_id = PartitionId.NotSet;
        static readonly CommittedEvent first_event = committed_events.single();
        static readonly EventProcessorId event_processor_id = Guid.NewGuid();

        static readonly Moq.Mock<IEventProcessor> event_processor_mock =
            Processing.given.an_event_processor(event_processor_id, (@event, partitionId, token) =>
                {
                    if (count == 2) return Task.FromResult<IProcessingResult>(new FailedProcessingResult(failure_reason));
                    count++;
                    return Task.FromResult<IProcessingResult>(new RetryProcessingResult(0, "retry reason"));
                });

        static int count;

        static StreamProcessor stream_processor;
        static Task task;

        Establish context = () =>
        {
            next_event_fetcher.Setup(_ => _.Fetch(Moq.It.IsAny<StreamId>(), 0, Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(new StreamEvent(first_event, Guid.NewGuid(), partition_id)));
            next_event_fetcher.Setup(_ => _.Fetch(Moq.It.IsAny<StreamId>(), 1, Moq.It.IsAny<CancellationToken>())).Throws(new Exception());
            next_event_fetcher.Setup(_ => _.FindNext(Moq.It.IsAny<StreamId>(), partition_id, 0, Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(new StreamPosition(0)));
            next_event_fetcher.Setup(_ => _.FindNext(Moq.It.IsAny<StreamId>(), partition_id, Moq.It.IsInRange(new StreamPosition(1), new StreamPosition(uint.MaxValue), Moq.Range.Inclusive), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(new StreamPosition(uint.MaxValue)));
            stream_processor = new StreamProcessor(tenant_id, source_stream_id, event_processor_mock.Object, stream_processor_states, next_event_fetcher.Object, default, Moq.Mock.Of<ILogger>());
        };

        Because of = () => stream_processor.BeginProcessing().Wait();

        It should_process_three_times = () => event_processor_mock.Verify(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
        It should_process_first_event_three_times = () => event_processor_mock.Verify(_ => _.Process(first_event, partition_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(3));
        It should_have_current_position_equal_zero = () => stream_processor.CurrentState.Position.ShouldEqual(new StreamPosition(1));
        It should_have_one_failing_partition = () => stream_processor.CurrentState.FailingPartitions.Count.ShouldEqual(1);
        It should_have_the_correct_failing_partition = () => stream_processor.CurrentState.FailingPartitions.ContainsKey(partition_id).ShouldBeTrue();
        It should_have_the_correct_position_on_the_failing_partition = () => stream_processor.CurrentState.FailingPartitions[partition_id].Position.ShouldEqual(new StreamPosition(0));
        It should_have_the_correct_reason_on_the_failing_partition = () => stream_processor.CurrentState.FailingPartitions[partition_id].Reason.ShouldEqual(failure_reason);
        It should_have_the_correct_retry_time_on_the_failing_partition = () => stream_processor.CurrentState.FailingPartitions[partition_id].RetryTime.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
    }
}