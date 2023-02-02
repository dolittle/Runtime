// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_TimeToRetryForPartitionedStreamProcessor;

public class when_there_are_no_failing_partitions
{
    static TimeToRetryForPartitionedStreamProcessor time_to_retry_getter;
    static StreamProcessorState state;
    static bool success;
    static TimeSpan time_to_retry;

    Establish context = () =>
    {
        time_to_retry_getter = new TimeToRetryForPartitionedStreamProcessor();
        state = new StreamProcessorState(ProcessingPosition.Initial, new Dictionary<PartitionId, FailingPartitionState>(), DateTimeOffset.UtcNow);
    };

    Because of = () => success = time_to_retry_getter.TryGetTimespanToRetry(state, out time_to_retry);

    It should_not_get_it = () => success.ShouldBeFalse();
    It should_return_the_max_value_of_time_span = () => time_to_retry.ShouldEqual(TimeSpan.MaxValue);
}