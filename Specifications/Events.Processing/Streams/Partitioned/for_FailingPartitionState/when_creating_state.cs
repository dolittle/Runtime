// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitionState;

public class when_creating_state
{
    static ProcessingPosition position;
    static string reason;
    static DateTimeOffset retry_time;
    static FailingPartitionState state;
    static uint processing_attempts;

    Establish context = () =>
    {
        position = ProcessingPosition.Initial;
        reason = "reason";
        retry_time = DateTimeOffset.Now;
        processing_attempts = 0;
    };

    Because of = () =>
        state = new FailingPartitionState(position.StreamPosition, position.EventLogPosition, retry_time, reason, processing_attempts, DateTimeOffset.UtcNow);

    It should_have_the_correct_position = () => state.Position.ShouldEqual(position);
    It should_have_the_correct_reason = () => state.Reason.ShouldEqual(reason);
    It should_have_the_correct_retry_time = () => state.RetryTime.ShouldEqual(retry_time);
    It should_have_the_correct_processing_attempts = () => state.ProcessingAttempts.ShouldEqual(processing_attempts);
}