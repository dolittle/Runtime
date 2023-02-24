// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Events.Processing.Tests.Partitioned.given;
using FluentAssertions;

namespace Events.Processing.Tests.Partitioned.with_failing_state;

public class when_skipping_event : state_with_failing_partititons
{
    const string reason = "Something went wrong";
    readonly StreamProcessorState after_state;


    public when_skipping_event()
    {
        after_state = before_state.WithResult(SkippedProcessing.Instance, next_event, now);
    }


    [Fact]
    public void ShouldIncrementStreamPositionOnSkip()
    {
        after_state.Position.Should().BeEquivalentTo(NextProcessingPosition);
    }

    [Fact]
    public void ShouldPopulateFailingPartitions()
    {
        after_state.FailingPartitions.Should().HaveCount(2);
    }

    [Fact]
    public void ShouldNotUpdateFailingPartition()
    {
        var existing = before_state.FailingPartitions[partition];
        var failing_partition = after_state.FailingPartitions[partition];
        failing_partition.Should().BeSameAs(existing);
    }

    [Fact]
    public void ShouldNotUpdateOtherFailingPartition()
    {
        var failing_partition = after_state.FailingPartitions[partition2];
        failing_partition.Should().BeSameAs(failing_partitions[partition2]);
    }

    [Fact]
    public void ShouldNotUpdateLastSuccessfulTimestamp()
    {
        after_state.LastSuccessfullyProcessed.Should().Be(last_successful_processing);
    }
}