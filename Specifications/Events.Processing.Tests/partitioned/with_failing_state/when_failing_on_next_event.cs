// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Events.Processing.Tests.Partitioned.given;
using FluentAssertions;

namespace Events.Processing.Tests.Partitioned.with_failing_state;

public class when_failing_on_next_event: state_with_failing_partititons
{
    private const string reason = "Something went wrong";
    readonly StreamProcessorState after_state;

    public when_failing_on_next_event()
    {
        after_state = before_state.WithResult(new FailedProcessing(reason, true, retry_timeout), next_event, now);
    }


    [Fact]
    public void ShouldIncrementStreamPositionFailedProcessing()
    {
        after_state.Position.Should().BeEquivalentTo(CurrentProcessingPosition.IncrementWithStream());
    }

    [Fact]
    public void ShouldHaveAndAdditionalFailedPartition()
    {
        after_state.FailingPartitions.Should().HaveCount(before_state.FailingPartitions.Count + 1);

    }
    
    [Fact]
    public void ShouldAddNewFailedPartition()
    {
        var failing_partition = after_state.FailingPartitions[partition3];
        failing_partition.Should().BeEquivalentTo(new FailingPartitionState(next_event.CurrentProcessingPosition, after_retry, reason, 1, now));

    }

    [Fact]
    public void ShouldNotUpdateLastSuccessfulTimestamp()
    {
        after_state.LastSuccessfullyProcessed.Should().Be(last_successful_processing);
    }
}