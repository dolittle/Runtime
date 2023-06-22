// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorState;

public class when_creating_failing_state
{
    static ProcessingPosition processing_position;
    static string failure_reason;
    static DateTimeOffset retry_time;
    static uint processing_attempts;
    static StreamProcessorState state;

    Establish context = () =>
    {
        processing_position = ProcessingPosition.Initial;
        failure_reason = "";
        retry_time = DateTimeOffset.UtcNow;
        processing_attempts = 0;
    };

    Because of = () => state = new StreamProcessorState(processing_position, failure_reason, retry_time, processing_attempts, DateTimeOffset.MinValue, false);

    It should_have_the_correct_stream_position = () => state.Position.ShouldEqual(processing_position);
    It should_have_the_correct_failure_reason = () => state.FailureReason.ShouldEqual(failure_reason);
    It should_have_the_correct_retry_time = () => state.RetryTime.ShouldEqual(retry_time);
    It should_have_the_correct_processing_attempts = () => state.ProcessingAttempts.ShouldEqual(processing_attempts);
}