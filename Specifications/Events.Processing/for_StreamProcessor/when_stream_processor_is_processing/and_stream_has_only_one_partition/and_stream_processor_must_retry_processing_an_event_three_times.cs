// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.for_StreamProcessor.when_stream_processor_is_processing.and_stream_has_only_one_partition
{
    public class and_stream_processor_must_retry_processing_an_event_three_times : given.all_dependencies
    {
        static readonly PartitionId partition_id = PartitionId.NotSet;
        static readonly Store.CommittedEvent first_event = Processing.given.a_committed_event;
        static readonly EventProcessorId event_processor_id = Guid.NewGuid();

        static readonly Moq.Mock<IEventProcessor> event_processor_mock =
            Processing.given.an_event_processor_mock(event_processor_id, (@event, partitionId) =>
                {
                    if (count == 2) return Task.FromResult<IProcessingResult>(new FailedProcessingResult());
                    count++;
                    return Task.FromResult<IProcessingResult>(new RetryProcessingResult(0));
                });

        static int count;

        static StreamProcessor stream_processor;
        static Task task;

        Establish context = () =>
        {
            next_event_fetcher.Setup(_ => _.Fetch(Moq.It.IsAny<StreamId>(), 0, Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(new CommittedEventWithPartition(first_event, partition_id)));
            next_event_fetcher.Setup(_ => _.Fetch(Moq.It.IsAny<StreamId>(), 1, Moq.It.IsAny<CancellationToken>())).Throws(new Exception());
            next_event_fetcher.Setup(_ => _.FindNext(Moq.It.IsAny<StreamId>(), partition_id, 0, Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(new StreamPosition(0)));
            next_event_fetcher.Setup(_ => _.FindNext(Moq.It.IsAny<StreamId>(), partition_id, Moq.It.IsInRange(new StreamPosition(1), new StreamPosition(uint.MaxValue), Moq.Range.Inclusive), Moq.It.IsAny<CancellationToken>())).Returns(Task.FromResult(new StreamPosition(uint.MaxValue)));
            stream_processor = new StreamProcessor(tenant_id, source_stream_id, event_processor_mock.Object, stream_processor_state_repository, next_event_fetcher.Object, Moq.Mock.Of<ILogger>());
        };

        Because of = () => stream_processor.BeginProcessing().Wait();

        It should_process_three_times = () => event_processor_mock.Verify(_ => _.Process(Moq.It.IsAny<Store.CommittedEvent>(), Moq.It.IsAny<PartitionId>()), Moq.Times.Exactly(3));
        It should_process_first_event_three_times = () => event_processor_mock.Verify(_ => _.Process(first_event, partition_id), Moq.Times.Exactly(3));
        It should_have_current_position_equal_zero = () => stream_processor.CurrentState.Position.ShouldEqual(new StreamPosition(1));
        It should_have_one_failing_partition = () => stream_processor.CurrentState.FailingPartitions.Count.ShouldEqual(1);
        It should_have_the_correct_failing_partition = () => stream_processor.CurrentState.FailingPartitions.ContainsKey(partition_id).ShouldBeTrue();
        It should_have_the_correct_position_on_the_failing_partition = () => stream_processor.CurrentState.FailingPartitions[partition_id].Position.ShouldEqual(new StreamPosition(0));
        It should_have_the_correct_retry_time_on_the_failing_partition = () => stream_processor.CurrentState.FailingPartitions[partition_id].RetryTime.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
    }
}