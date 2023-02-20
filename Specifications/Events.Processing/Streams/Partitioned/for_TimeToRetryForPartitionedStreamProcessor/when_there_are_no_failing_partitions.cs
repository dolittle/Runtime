// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_TimeToRetryForPartitionedStreamProcessor;

public class when_there_are_no_failing_partitions
{
    static StreamProcessorState state;
    static bool success;
    static TimeSpan time_to_retry;

    Establish context = () =>
    {
        state = new StreamProcessorState(ProcessingPosition.Initial, ImmutableDictionary<PartitionId, FailingPartitionState>.Empty, DateTimeOffset.UtcNow);
    };

    Because of = () => success = state.TryGetTimespanToRetry(out time_to_retry);

    It should_not_get_it = () => success.ShouldBeFalse();
    It should_return_the_max_value_of_time_span = () => time_to_retry.ShouldEqual(TimeSpan.MaxValue);
}