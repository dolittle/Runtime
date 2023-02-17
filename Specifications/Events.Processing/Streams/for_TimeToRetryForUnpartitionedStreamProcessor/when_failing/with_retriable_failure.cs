// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Processing.Streams.for_TimeToRetryForUnpartitionedStreamProcessor.when_failing;

public class with_retriable_failure
{
    static StreamProcessorState before;
    static IStreamProcessorState state;
    static bool success;
    static TimeSpan time_to_retry;
    static DateTimeOffset at;
    static StreamEvent stream_event;



    private Establish context = () =>
    {
        stream_event = new StreamEvent(committed_events.single(), 0, Guid.Empty, "partition", false);

        var position = ProcessingPosition.Initial.IncrementWithStream();
        
        at = DateTimeOffset.UtcNow;
        before = new StreamProcessorState(ProcessingPosition.Initial, DateTimeOffset.UtcNow);
        var failedProcessing = new FailedProcessing("testing", true, TimeSpan.FromSeconds(5));
        state = ((IStreamProcessorState)before).WithResult(failedProcessing, stream_event, at);

    };

    Because of = () => success = state.TryGetTimespanToRetry(out time_to_retry);

    It should_get_it = () => success.ShouldBeTrue();
    It should_get_the_correct_delay = () => time_to_retry.ShouldBeGreaterThan(TimeSpan.FromSeconds(4));
    It should_not_be_higher_than_original_delay = () => time_to_retry.ShouldBeLessThan(TimeSpan.FromSeconds(5));
}