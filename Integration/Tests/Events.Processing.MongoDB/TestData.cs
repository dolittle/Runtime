// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store.Streams;

namespace Events.Processing.MongoDB;

static class test_data
{
    public static StreamProcessorState clean_partitioned_state()
    {
        var now = now_truncated();

        var partitionedState = new StreamProcessorState(ProcessingPosition.Initial.IncrementWithStream(), now);
        return partitionedState;
    }
    
    public static StreamProcessorState failing_partitioned_state()
    {
        var now = now_truncated();
        var failingPartitions = new Dictionary<PartitionId, FailingPartitionState>()
        {
            { new PartitionId("1"), new FailingPartitionState(ProcessingPosition.Initial.IncrementWithStream(), now.AddMinutes(5), "Something failed", 2, now) },
        }.ToImmutableDictionary();

        var partitionedState = new StreamProcessorState(ProcessingPosition.Initial.IncrementWithStream().IncrementWithStream(), failingPartitions, now);
        return partitionedState;
    }

    public static Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState clean_non_partitioned_state()
    {
        var now = now_truncated();
        var state = new Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState(ProcessingPosition.Initial.IncrementWithStream(), now);
        return state;
    }

    public static Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState failing_non_partitioned_state()
    {
        var now = now_truncated();
        var state = new Dolittle.Runtime.Events.Processing.Streams.StreamProcessorState(ProcessingPosition.Initial.IncrementWithStream(), "Something failed",
            now.AddMinutes(5), 2, now, true);
        return state;
    }

    // Truncates ticks to milliseconds, as MongoDB does not support storing ticks
    static DateTimeOffset now_truncated()
    {
        var now = DateTimeOffset.UtcNow;
        return new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, now.Offset);
    }
}