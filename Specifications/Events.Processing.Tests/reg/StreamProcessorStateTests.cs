﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;

namespace Events.Processing.Tests.NonPartitioned;

public class StreamProcessorStateTests
{
    private const string Partition = "partition";
    readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    static readonly StreamEvent Evt = new(
        committed_events.single(EventLogSequenceNumber.Initial),
        StreamPosition.Start,
        StreamId.EventLog,
        Partition,
        true);

    [Fact]
    public void ShouldIncrementStreamPositionOnSuccessFulProcessing()
    {
        var before = StreamProcessorState.New;
        var afterState = before.WithSuccessfullyProcessed(Evt, Now);

        afterState.Should().BeEquivalentTo(new StreamProcessorState(Evt.NextProcessingPosition, Now));
    }

    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(10, 10)]
    [InlineData(100, 100)]
    [InlineData(-1, 0)]
    [InlineData(-10, 0)]
    public void should_skip_correct_number_of_events(long relativePosition, ulong expectedSkipped)
    {
        var before_state = StreamProcessorState.New.WithSuccessfullyProcessed(Evt, Now);

        var beforePosition = (long)before_state.Position.EventLogPosition.Value;
        
        var targetPosition = beforePosition + relativePosition;
        if(targetPosition < 0) targetPosition = 0;
        EventLogSequenceNumber targetEventLogSequenceNumber = new EventLogSequenceNumber((ulong)targetPosition);

        var after = before_state.SkipEventsBefore(targetEventLogSequenceNumber);
        
        after.FailingPartitionCount.Should().Be(0);

        after.EarliestProcessingPosition.StreamPosition.Should().Be(before_state.Position.StreamPosition); // No change
        after.EarliestProcessingPosition.EventLogPosition.Should().Be(before_state.Position.EventLogPosition + expectedSkipped);
        
        after.Position.StreamPosition.Should().Be(before_state.Position.StreamPosition); // No change
        after.Position.EventLogPosition.Should().Be(before_state.Position.EventLogPosition + expectedSkipped);

        if (expectedSkipped == 0)
        {
            after.Should().BeSameAs(before_state);
        }
    }
}