// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store.Streams;

namespace Events.Processing.Tests.Partitioned.given;

public class state_with_failing_partititons
{
    protected const string partition = "partition";
    protected const string partition2 = "partition2";
    protected const string partition3 = "partition3";
    protected const string failure_reason = "Something went wrong";
    protected const string failure_reason_2 = "Something went wrong again";
    protected static readonly DateTimeOffset original_failure_time = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(20);
    protected static readonly DateTimeOffset failure_time2 = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(15);
    protected static readonly DateTimeOffset last_successful_processing = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(10);
    protected static readonly DateTimeOffset now = DateTimeOffset.UtcNow;
    
    protected static readonly TimeSpan retry_timeout = TimeSpan.FromSeconds(5);
    protected static readonly DateTimeOffset after_retry = now.Add(retry_timeout);

    protected static readonly ProcessingPosition FailingProcessingPosition = new(18, 36);
    protected static readonly ProcessingPosition FailingProcessingPosition2 = new(20, 38);
    protected static readonly ProcessingPosition CurrentProcessingPosition = new(22, 42);

    protected static readonly StreamEvent retry_event = new(
        committed_events.single(FailingProcessingPosition.EventLogPosition),
        FailingProcessingPosition.StreamPosition,
        StreamId.EventLog,
        partition,
        true);
    
    protected static readonly StreamEvent next_event = new(
        committed_events.single(CurrentProcessingPosition.EventLogPosition),
        CurrentProcessingPosition.StreamPosition,
        StreamId.EventLog,
        partition3,
        true);

    protected static readonly ImmutableDictionary<PartitionId, FailingPartitionState> failing_partitions = new Dictionary<PartitionId, FailingPartitionState>()
    {
        { partition, new FailingPartitionState(FailingProcessingPosition, after_retry, failure_reason, 1, original_failure_time) },
        { partition2, new FailingPartitionState(FailingProcessingPosition2, after_retry, failure_reason_2, 1, failure_time2) }
    }.ToImmutableDictionary();

    protected static readonly StreamProcessorState before_state = new StreamProcessorState(CurrentProcessingPosition, failing_partitions, last_successful_processing);

    
    
}