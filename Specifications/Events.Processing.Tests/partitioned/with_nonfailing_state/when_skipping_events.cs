// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using FluentAssertions;

namespace Events.Processing.Tests.Partitioned;

public class when_skipping_events: given.non_failing_state
{
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    [InlineData(100, 100)]
    [InlineData(0, 0)]
    [InlineData(-1, 0)]
    [InlineData(-10, 0)]
    public void should_skip_correct_number_of_events(long relativePosition, ulong expectedSkipped)
    {
        var beforePosition = (long)before_state.Position.EventLogPosition.Value;
        
        var targetPosition = beforePosition + relativePosition;
        if(targetPosition < 0) targetPosition = 0;
        var targetEventLogSequenceNumber = new EventLogSequenceNumber((ulong)targetPosition);

        var after = before_state.SkipEventsBefore(targetEventLogSequenceNumber);
        
        after.FailingPartitionCount.Should().Be(0);

        after.EarliestProcessingPosition.StreamPosition.Should().Be(before_state.Position.StreamPosition); // No change
        after.EarliestProcessingPosition.EventLogPosition.Should().Be(before_state.Position.EventLogPosition + expectedSkipped);
        
        after.Position.StreamPosition.Should().Be(before_state.Position.StreamPosition); // No change
        after.Position.EventLogPosition.Should().Be(before_state.Position.EventLogPosition + expectedSkipped);

        if (expectedSkipped == 0) // No change, should be same instance
        {
            after.Should().BeSameAs(before_state);
        }
    }
}