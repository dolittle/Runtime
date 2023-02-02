// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_TimeToRetryForUnpartitionedStreamProcessor.when_failing;

public class and_retry_time_is_in_the_future
{
    static TimeToRetryForUnpartitionedStreamProcessor time_to_retry_getter;
    static StreamProcessorState state;
    static bool success;
    static TimeSpan time_to_retry;

    Establish context = () =>
    {
        time_to_retry_getter = new TimeToRetryForUnpartitionedStreamProcessor();
        state = new StreamProcessorState(ProcessingPosition.Initial, "reason", DateTimeOffset.UtcNow.AddSeconds(60), 0, DateTimeOffset.UtcNow, true);
    };

    Because of = () => success = time_to_retry_getter.TryGetTimespanToRetry(state, out time_to_retry);

    It should_get_it = () => success.ShouldBeTrue();
    It should_retry_in_the_future = () => time_to_retry.ShouldBeGreaterThan(TimeSpan.Zero);
}