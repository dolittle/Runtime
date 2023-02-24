// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Events.Processing.Tests.Partitioned.given;
using FluentAssertions;

namespace Events.Processing.Tests.Partitioned.with_failing_state;

public class when_processing_next_event: state_with_failing_partititons
{
    private const string reason = "Something went wrong";
    readonly StreamProcessorState after_state;

    public when_processing_next_event()
    {
        after_state = before_state.WithResult(SuccessfulProcessing.Instance, next_event, now);
    }


    [Fact]
    public void ShouldIncrementStreamPositionOnSuccessFulProcessing()
    {
        after_state.Position.Should().BeEquivalentTo(CurrentProcessingPosition.IncrementWithStream());
    }

    [Fact]
    public void ShouldNotChangeFailingPartitions()
    {
        after_state.FailingPartitions.Should().BeSameAs(before_state.FailingPartitions);

    }

    [Fact]
    public void ShouldUpdateLastSuccessfulTimestamp()
    {
        after_state.LastSuccessfullyProcessed.Should().Be(now);
    }
}