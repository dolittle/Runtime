// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;

namespace Events.Processing.Tests.Partitioned;

public class when_successfully_processed: given.non_failing_state
{
    readonly StreamProcessorState after_state;
    
    public when_successfully_processed()
    {
        after_state = before_state.WithResult(SuccessfulProcessing.Instance, Evt, now);
    }


    [Fact]
    public void ShouldIncrementStreamPositionOnSuccessFulProcessing()
    {
        after_state.Position.Should().BeEquivalentTo(CurrentProcessingPosition.IncrementWithStream());
    }
    
    [Fact]
    public void ShouldNotPopulateFailingPartitions()
    {
        after_state.FailingPartitions.Should().BeEmpty();
    }

    [Fact]
    public void ShouldUpdateLastSuccessfulTimestamp()
    {
        after_state.LastSuccessfullyProcessed.Should().Be(now);
    }
}