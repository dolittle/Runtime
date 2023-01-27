// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_TimeToRetryForPartitionedStreamProcessor.when_there_are_failing_partitions;

public class and_earliest_retry_time_is_in_the_past
{
    static TimeToRetryForPartitionedStreamProcessor time_to_retry_getter;
    static IDictionary<PartitionId, FailingPartitionState> failing_partitions;
    static StreamProcessorState state;
    static bool success;
    static TimeSpan time_to_retry;

    Establish context = () =>
    {
        time_to_retry_getter = new TimeToRetryForPartitionedStreamProcessor();
        failing_partitions = new Dictionary<PartitionId, FailingPartitionState>()
        {
            {
                "9b025684-130c-4789-b904-878a8e26c2d0",
                new FailingPartitionState(
                    0,
                    DateTimeOffset.UtcNow.AddSeconds(-10),
                    "reason",
                    0,
                    DateTimeOffset.UtcNow)
            },
            {
                "e90f7d42-0a21-4d90-93a8-e56bca822b67",
                new FailingPartitionState(
                    0,
                    DateTimeOffset.UtcNow.AddSeconds(64),
                    "reason",
                    0,
                    DateTimeOffset.UtcNow)
            },
            {
                "36975349-318d-4c01-bbbb-4f1aeaf4e7ea",
                new FailingPartitionState(
                    0,
                    DateTimeOffset.UtcNow.AddSeconds(78),
                    "reason",
                    0,
                    DateTimeOffset.UtcNow)
            }
        };
        state = new StreamProcessorState(0, failing_partitions, DateTimeOffset.UtcNow);
    };

    Because of = () => success = time_to_retry_getter.TryGetTimespanToRetry(state, out time_to_retry);

    It should_get_it = () => success.Should().BeTrue();
    It should_retry_now = () => time_to_retry.Should().Be(TimeSpan.Zero);
}