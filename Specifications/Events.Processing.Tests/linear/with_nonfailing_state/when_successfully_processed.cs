// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using FluentAssertions;

namespace Events.Processing.Tests;

public class when_successfully_processed: given.non_failing_state
{
    readonly StreamProcessorState after_state;
    
    public when_successfully_processed()
    {
        after_state = before_state.WithResult(SuccessfulProcessing.Instance, Evt, now);
    }

    [Fact]
    public void ShouldIncrementStreamPositionOnSuccessfulRetry()
    {
        after_state.Position.Should().BeEquivalentTo(NextProcessingPosition);
    }


    [Fact]
    public void ShouldNotBeFailing()
    {
        after_state.IsFailing.Should().BeFalse();
    }

    [Fact]
    public void ShouldClearReason()
    {
        after_state.FailureReason.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ShouldClearProcessingAttempts()
    {
        after_state.ProcessingAttempts.Should().Be(0);
    }

    [Fact]
    public void ShouldUpdateLastSuccessfulTimestamp()
    {
        after_state.LastSuccessfullyProcessed.Should().Be(now);
    }
}