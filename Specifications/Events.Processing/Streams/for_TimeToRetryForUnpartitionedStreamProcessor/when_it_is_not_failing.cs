// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_TimeToRetryForUnpartitionedStreamProcessor
{
    public class when_it_is_not_failing
    {
        static TimeToRetryForUnpartitionedStreamProcessor time_to_retry_getter;
        static StreamProcessorState state;
        static bool success;
        static TimeSpan time_to_retry;

        Establish context = () =>
        {
            time_to_retry_getter = new TimeToRetryForUnpartitionedStreamProcessor();
            state = new StreamProcessorState(0, DateTimeOffset.UtcNow);
        };

        Because of = () => success = time_to_retry_getter.TryGetTimespanToRetry(state, out time_to_retry);

        It should_not_get_it = () => success.ShouldBeFalse();
        It should_return_the_max_value_of_time_span = () => time_to_retry.ShouldEqual(TimeSpan.MaxValue);
    }
}