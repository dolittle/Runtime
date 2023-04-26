// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Events.Processing.Tests.given;

public class failing_state
{
    protected const string partition = "partition";
    protected const string failure_reason = "Something went wrong";
    protected const string failure_reason_2 = "Something went wrong again";
    protected static readonly DateTimeOffset original_failure_time = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(20);
    protected static readonly DateTimeOffset original_retry_time = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(1);
    protected static readonly DateTimeOffset last_successful_processing = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(10);
    protected static readonly DateTimeOffset now = DateTimeOffset.UtcNow;

    protected static readonly TimeSpan retry_timeout = TimeSpan.FromSeconds(5);
    protected static readonly DateTimeOffset after_retry = now.Add(retry_timeout);

    protected static readonly ProcessingPosition CurrentProcessingPosition = new(22, 42);
    protected static readonly ProcessingPosition NextProcessingPosition = CurrentProcessingPosition.IncrementWithStream();

    protected static readonly StreamEvent current_event = new(
        committed_events.single(CurrentProcessingPosition.EventLogPosition),
        CurrentProcessingPosition.StreamPosition,
        StreamId.EventLog,
        partition,
        true);


    protected static readonly StreamProcessorState before_state = new(CurrentProcessingPosition, failure_reason, original_retry_time, 1,
        last_successful_processing, true);
}