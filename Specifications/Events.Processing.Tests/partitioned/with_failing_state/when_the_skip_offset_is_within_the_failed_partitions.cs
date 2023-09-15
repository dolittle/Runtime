// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store;
using FluentAssertions;

namespace Events.Processing.Tests.Partitioned.with_failing_state;

public class when_the_skip_offset_is_within_the_failed_partitions: given.state_with_failing_partititons
{
    static readonly EventLogSequenceNumber target_position = new(37);
    StreamProcessorState after_state = before_state.SkipEventsBefore(target_position);
    
    [Fact]
    public void should_have_the_correct_number_of_failing_partitions() => after_state.FailingPartitionCount.Should().Be(2);
    
    [Fact]
    public void Should_not_mutate_the_high_watermark() => after_state.Position.Should().Be(before_state.Position,"No change to the high watermark");
    
    [Fact]
    public void should_have_the_correct_earliest_processing_position() => after_state.EarliestProcessingPosition.EventLogPosition.Should().Be(target_position);

    [Fact]
    public void should_have_updated_the_first_partition()
    {
        var before = before_state.FailingPartitions[partition];
        var after = after_state.FailingPartitions[partition];
        
        after.Position.EventLogPosition.Should().Be(target_position);
        after.Position.StreamPosition.Should().Be(before.Position.StreamPosition);
        after.RetryTime.Should().Be(before.RetryTime);
        after.Reason.Should().Be("Skipped older events in the partition");
        after.ProcessingAttempts.Should().Be(0);
    }
    
    [Fact]
    public void should_have_kept_the_second_partition()
    {
        var before = before_state.FailingPartitions[partition2];
        var after = after_state.FailingPartitions[partition2];

        after.Should().BeSameAs(before, "The partition was not affected by the skip");
    }
    
}