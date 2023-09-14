// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using FluentAssertions;

namespace Events.Processing.Tests.Partitioned.with_failing_state;

public class when_skipping_events: given.state_with_failing_partititons
{
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    [InlineData(100, 100)]
    public void and_the_offset_is_later_or_at_the_high_watermark(long relativePosition, ulong expectedSkipped)
    {
        var beforePosition = (long)before_state.Position.EventLogPosition.Value;
        
        var targetPosition = beforePosition + relativePosition;
        if(targetPosition < 0) targetPosition = 0;
        var targetEventLogSequenceNumber = new EventLogSequenceNumber((ulong)targetPosition);

        var after = before_state.SkipEventsBefore(targetEventLogSequenceNumber);
        
        after.FailingPartitionCount.Should().Be(0,"Partitions should be reset since we are ahead of the high watermark");

        after.EarliestProcessingPosition.EventLogPosition.Should().Be(before_state.Position.EventLogPosition + expectedSkipped);
        
        after.Position.StreamPosition.Should().Be(before_state.Position.StreamPosition); // No change
        after.Position.EventLogPosition.Should().Be(before_state.Position.EventLogPosition + expectedSkipped);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void and_the_offset_is_before_the_failed_partitions(long relativePosition)
    {
        var beforePosition = (long)before_state.EarliestProcessingPosition.EventLogPosition.Value;
        
        var targetPosition = beforePosition + relativePosition;
        if(targetPosition < 0) targetPosition = 0;
        var targetEventLogSequenceNumber = new EventLogSequenceNumber((ulong)targetPosition);

        var after = before_state.SkipEventsBefore(targetEventLogSequenceNumber);
        after.Should().BeSameAs(before_state, "There is no change so the same instance should be returned");
    }

    [Fact]
    public void and_the_offset_is_within_the_failed_partitions()
    {
        var target_position = new EventLogSequenceNumber(37);
        var after = before_state.SkipEventsBefore(target_position);
        
        after.FailingPartitionCount.Should().Be(2);
        after.EarliestProcessingPosition.EventLogPosition.Should().Be(target_position);
        after.Position.Should().Be(before_state.Position,"No change to the high watermark");

        // Partition2
        after.FailingPartitions[partition2].Should().BeSameAs(before_state.FailingPartitions[partition2]);
        
        var updatedPartition = after.FailingPartitions[partition];
    }
    
    
}