// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Events.Processing.Tests.Partitioned.given;
using FluentAssertions;

namespace Events.Processing.Tests.Partitioned.with_failing_state.without_retries;

public class when_retry_successful: state_with_failing_partititons
{
    private const string reason = "Something went wrong";
    readonly StreamProcessorState after_state;
    private DateTimeOffset later = now.AddSeconds(10);

    public when_retry_successful()
    {
        after_state = before_state.WithResult(SuccessfulProcessing.Instance, retry_event, now);
    }


    [Fact]
    public void ShouldNotIncrementStreamPositionOnSuccessfulRetry()
    {
        after_state.Position.Should().BeEquivalentTo(CurrentProcessingPosition);
    }

    [Fact]
    public void ShouldPopulateFailingPartitions()
    {
        after_state.FailingPartitions.Should().HaveCount(2);
    }

    [Fact]
    public void ShouldUpdateCorrectFailingPartition()
    {
        var failing_partition = after_state.FailingPartitions[partition];
        failing_partition.Should().BeEquivalentTo(new FailingPartitionState(retry_event.NextProcessingPosition, now, "behind", 0, original_failure_time));
    }
    
    [Fact]
    public void ShouldNotUpdateOtherFailingPartition()
    {
        var failing_partition = after_state.FailingPartitions[partition2];
        failing_partition.Should().BeSameAs(failing_partitions[partition2]);
    }

    [Fact]
    public void ShouldUpdateLastSuccessfulTimestamp()
    {
        after_state.LastSuccessfullyProcessed.Should().Be(now);
    }
}