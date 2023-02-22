// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.when_catching_up.and_there_are_multiple_failing_partitions.given;

public class all_dependencies : when_catching_up.given.all_dependencies
{
    protected static PartitionId first_failing_partition_id;
    protected static PartitionId second_failing_partition_id;
    protected static ProcessingPosition first_initial_failing_partition_position;
    protected static ProcessingPosition second_initial_failing_partition_position;
    protected static string first_initial_failing_partition_reason;
    protected static string second_initial_failing_partition_reason;
    protected static DateTimeOffset first_initial_failing_partition_retry_time;
    protected static DateTimeOffset second_initial_failing_partition_retry_time;
    protected static FailingPartitionState first_failing_partition_state;
    protected static FailingPartitionState second_failing_partition_state;

    Establish context = () =>
    {
        first_failing_partition_id = "first failing partition";
        second_failing_partition_id = "the second failing partiton";
        first_initial_failing_partition_position = second_initial_failing_partition_position = ProcessingPosition.Initial;
        first_initial_failing_partition_reason = second_initial_failing_partition_reason = "some reason";
        first_initial_failing_partition_retry_time = second_initial_failing_partition_retry_time = DateTimeOffset.UtcNow;
        first_failing_partition_state = new FailingPartitionState(first_initial_failing_partition_position, first_initial_failing_partition_retry_time, first_initial_failing_partition_reason, 1, DateTimeOffset.MinValue);
        second_failing_partition_state = new FailingPartitionState(second_initial_failing_partition_position, second_initial_failing_partition_retry_time, second_initial_failing_partition_reason, 1, DateTimeOffset.MinValue);

        stream_processor_state = stream_processor_state with
        {
            FailingPartitions = stream_processor_state.FailingPartitions
                .Add(first_failing_partition_id, first_failing_partition_state)
                .Add(second_failing_partition_id, second_failing_partition_state)
        };

        eventStream = new[]
        {
            new StreamEvent(committed_events.single(), 0, stream_id, first_failing_partition_id, true),
            new StreamEvent(committed_events.single(1), 1, stream_id, second_failing_partition_id, true),
            new StreamEvent(committed_events.single(2), 2, stream_id, first_failing_partition_id, true),
        };
    };
}