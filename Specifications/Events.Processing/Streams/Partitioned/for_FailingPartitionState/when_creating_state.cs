// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitionState;

public class when_creating_state
{
    static StreamPosition position;
    static string reason;
    static DateTimeOffset retry_time;
    static FailingPartitionState state;
    static uint processing_attempts;

    Establish context = () =>
    {
        position = 0;
        reason = "reason";
        retry_time = DateTimeOffset.Now;
        processing_attempts = 0;
    };

    Because of = () =>
        state = new FailingPartitionState(position, retry_time, reason, processing_attempts, DateTimeOffset.UtcNow);

    It should_have_the_correct_position = () => state.Position.Should().Be(position);
    It should_have_the_correct_reason = () => state.Reason.Should().Be(reason);
    It should_have_the_correct_retry_time = () => state.RetryTime.Should().Be(retry_time);
    It should_have_the_correct_processing_attempts = () => state.ProcessingAttempts.Should().Be(processing_attempts);
}