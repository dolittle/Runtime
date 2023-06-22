// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Events.Processing.Tests.given;
using FluentAssertions;

namespace Events.Processing.Tests.with_failing_state;

public class when_failing_again : failing_state
{
    readonly StreamProcessorState after_state;

    public when_failing_again()
    {
        after_state = before_state.WithResult(new FailedProcessing(failure_reason_2, true, retry_timeout), current_event, now);
    }


    [Fact]
    public void ShouldNotIncrementStreamPositionOnFailedRetry()
    {
        after_state.Position.Should().BeEquivalentTo(CurrentProcessingPosition);
    }

    [Fact]
    public void ShouldUpdateProcessingAttempts()
    {
        after_state.ProcessingAttempts.Should().Be(before_state.ProcessingAttempts + 1);
    }
    
    [Fact]
    public void ShouldSetRetryTimeout()
    {
        after_state.RetryTime.Should().Be(after_retry);
    }
    
    [Fact]
    public void ShouldUpdateFailureReason()
    {
        after_state.FailureReason.Should().Be(failure_reason_2);
    }

    [Fact]
    public void ShouldBeFailing()
    {
        after_state.IsFailing.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotUpdateLastSuccessfulTimestamp()
    {
        after_state.LastSuccessfullyProcessed.Should().Be(last_successful_processing);
    }
}