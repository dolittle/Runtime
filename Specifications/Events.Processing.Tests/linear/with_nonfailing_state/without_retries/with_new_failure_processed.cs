// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using FluentAssertions;

namespace Events.Processing.Tests.with_failing_state.without_retries;

public class with_new_failure_processed : given.non_failing_state
{
    private const string failure_reason = "Something went wrong";
    readonly StreamProcessorState after_state;

    public with_new_failure_processed()
    {
        after_state = before_state.WithResult(new FailedProcessing(failure_reason), Evt, now);
    }


    [Fact]
    public void ShouldNotIncrementStreamPositionOnFailedRetry()
    {
        after_state.Position.Should().BeEquivalentTo(CurrentProcessingPosition);
    }

    [Fact]
    public void ShouldSetProcessingAttempts()
    {
        after_state.ProcessingAttempts.Should().Be(1);
    }
    
    [Fact]
    public void ShouldSetRetryTimeout()
    {
        after_state.RetryTime.Should().Be(DateTimeOffset.MaxValue);
    }
    
    [Fact]
    public void ShouldBeNotRetryable()
    {
        after_state.TryGetTimespanToRetry(out _).Should().BeFalse();
    }
    
    [Fact]
    public void ShouldSetFailureReason()
    {
        after_state.FailureReason.Should().Be(failure_reason);
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