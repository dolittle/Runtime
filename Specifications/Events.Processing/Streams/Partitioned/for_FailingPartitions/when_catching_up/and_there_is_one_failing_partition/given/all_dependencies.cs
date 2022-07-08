// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up.and_there_is_one_failing_partition.given;

public class all_dependencies : when_catching_up.given.all_dependencies
{
    protected static PartitionId failing_partition_id;
    protected static StreamPosition initial_failing_partition_position;
    protected static string initial_failing_partition_reason;
    protected static DateTimeOffset initial_failing_partition_retry_time;
    protected static FailingPartitionState failing_partition_state;

    Establish context = () =>
    {
        failing_partition_id = "failing partition";
        initial_failing_partition_position = 0;
        initial_failing_partition_reason = "some reason";
        initial_failing_partition_retry_time = DateTimeOffset.UtcNow;
        failing_partition_state = new FailingPartitionState(initial_failing_partition_position, initial_failing_partition_retry_time, initial_failing_partition_reason, 1, DateTimeOffset.UtcNow);
        stream_processor_state.FailingPartitions.Add(failing_partition_id, failing_partition_state);
        eventStream = new[]
        {
            new StreamEvent(committed_events.single(), 0, stream_id, failing_partition_id, true),
            new StreamEvent(committed_events.single(), 1, stream_id, failing_partition_id, true),
            new StreamEvent(committed_events.single(), 2, stream_id, failing_partition_id, true),
        };
    };
}