// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Events.Processing.Tests.given;
using FluentAssertions;

namespace Events.Processing.Tests.with_failing_state;

public class when_retry_successful : failing_state
{
    readonly StreamProcessorState after_state;

    public when_retry_successful()
    {
        after_state = before_state.WithResult(SuccessfulProcessing.Instance, current_event, now);
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