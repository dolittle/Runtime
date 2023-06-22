// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using FluentAssertions;

namespace Events.Processing.Tests.Partitioned.with_failing_state.without_retries;

public class with_new_failure_processed : given.non_failing_state
{
    private const string reason = "Something went wrong";
    readonly StreamProcessorState after_state;
    private DateTimeOffset later = now.AddSeconds(10);

    public with_new_failure_processed()
    {
        after_state = before_state.WithResult(new FailedProcessing(reason), Evt, now);
    }


    [Fact]
    public void ShouldIncrementStreamPositionOnFailedProcessing()
    {
        after_state.Position.Should().BeEquivalentTo(CurrentProcessingPosition.IncrementWithStream());
    }

    [Fact]
    public void ShouldPopulateFailingPartitions()
    {
        after_state.FailingPartitions.Should().HaveCount(1);
    }

    [Fact]
    public void ShouldPopulateCorrectFailingPartition()
    {
        var failing_partition = after_state.FailingPartitions[partition];
        failing_partition.Should().BeEquivalentTo(new FailingPartitionState(Evt.CurrentProcessingPosition, DateTimeOffset.MaxValue, reason, 1, now));
    }

    [Fact]
    public void ShouldNotUpdateLastSuccessfulTimestamp()
    {
        after_state.LastSuccessfullyProcessed.Should().Be(last_successful_processing);
    }
}