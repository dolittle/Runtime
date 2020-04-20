// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_FailingPartitions.when_catching_up.and_there_is_one_failing_partition.given
{
    public class all_dependencies : when_catching_up.given.all_dependencies
    {
        protected static PartitionId failing_partition_id;
        protected static StreamPosition initial_failing_partition_position;
        protected static string initial_failing_partition_reason;
        protected static DateTimeOffset initial_failing_partition_retry_time;
        protected static FailingPartitionState failing_partition_state;

        Establish context = () =>
        {
            failing_partition_id = Guid.NewGuid();
            initial_failing_partition_position = 0;
            initial_failing_partition_reason = "some reason";
            initial_failing_partition_retry_time = DateTimeOffset.UtcNow;
            failing_partition_state = new FailingPartitionState
            {
                Position = initial_failing_partition_position,
                Reason = initial_failing_partition_reason,
                RetryTime = initial_failing_partition_retry_time
            };
            stream_processor_state.FailingPartitions.Add(failing_partition_id, failing_partition_state);

            events_fetcher
                .Setup(_ => _.FindNext(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<PartitionId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<StreamId, PartitionId, StreamPosition, CancellationToken>((stream, partition, position, _) =>
                {
                    if (position.Value >= initial_stream_processor_position) return Task.FromResult(new StreamPosition(uint.MaxValue));
                    return Task.FromResult(position);
                });
        };
    }
}