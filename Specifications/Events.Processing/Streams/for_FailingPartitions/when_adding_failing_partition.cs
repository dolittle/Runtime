// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions
{
    public class when_adding_failing_partition : given.an_instance_of_failing_partitions
    {
        static StreamProcessorId stream_processor_id;
        static PartitionId partition;
        static StreamPosition stream_position;
        static DateTimeOffset retry_time;
        static string reason;

        Establish context = () =>
        {
            stream_processor_id = new StreamProcessorId(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            partition = Guid.NewGuid();
            stream_position = 0;
            retry_time = DateTimeOffset.Now;
            reason = "";
        };

        Because of = () => failing_partitions.AddFailingPartitionFor(stream_processor_id, partition, stream_position, retry_time, reason, CancellationToken.None);

        It should_add_the_failing_partition_state = () => stream_processor_state_repository.Verify(_ => _.AddFailingPartition(stream_processor_id, partition, stream_position, retry_time, reason, Moq.It.IsAny<CancellationToken>()));
    }
}