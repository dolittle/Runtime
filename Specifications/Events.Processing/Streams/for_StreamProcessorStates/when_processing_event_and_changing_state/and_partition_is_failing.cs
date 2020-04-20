// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorStates.when_processing_event_and_changing_state
{
    public class and_partition_is_failing : given.all_dependencies
    {
        static PartitionId partition;
        static StreamProcessorId stream_processor_id;

        Establish context = () =>
        {
            partition = Guid.NewGuid();
            stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid());
        };

        Because of = () => stream_processor_states.ProcessEventAndChangeStateFor(
            stream_processor_id,
            event_processor.Object,
            given.stream_event.single(partition),
            new StreamProcessorState(0, new Dictionary<PartitionId, FailingPartitionState>
            {
                { partition, new FailingPartitionState() }
            })).GetAwaiter().GetResult();

        It should_increment_stream_processor_position = () => stream_processor_state_repository.Verify(_ => _.IncrementPosition(stream_processor_id, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    }
}