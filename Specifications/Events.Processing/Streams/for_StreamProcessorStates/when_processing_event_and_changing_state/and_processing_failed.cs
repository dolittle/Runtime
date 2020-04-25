// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorStates.when_processing_event_and_changing_state
{
    public class and_processing_failed : given.all_dependencies
    {
        static string failure_reason;
        static StreamEvent stream_event;
        static PartitionId partition;
        static StreamProcessorId stream_processor_id;

        Establish context = () =>
        {
            failure_reason = "reason";
            partition = Guid.NewGuid();
            stream_event = given.stream_event.single(partition);
            stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            event_processor
                .Setup(_ => _.Process(Moq.It.IsAny<CommittedEvent>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IProcessingResult>(new FailedProcessing(failure_reason)));
        };

        Because of = () => stream_processor_states.ProcessEventAndChangeStateFor(
            stream_processor_id,
            event_processor.Object,
            stream_event,
            new StreamProcessorState(0, new Dictionary<PartitionId, FailingPartitionState>()),
            CancellationToken.None).GetAwaiter().GetResult();

        It should_process_the_event = () => event_processor.Verify(_ => _.Process(stream_event.Event, partition, Moq.It.IsAny<CancellationToken>()));
        It should_add_failing_partition = () => failing_partitions.Verify(_ => _.AddFailingPartitionFor(stream_processor_id, partition, 0, DateTimeOffset.MaxValue, failure_reason, Moq.It.IsAny<CancellationToken>()));
        It should_increment_stream_processor_position = () => stream_processor_state_repository.Verify(_ => _.SetNextEventToProcessPosition(stream_processor_id, 1, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}