// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Events.Processing.Tests.given;

public class non_failing_state
{
    protected const string partition = "partition";
    protected static readonly DateTimeOffset last_successful_processing = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(10);
    protected static readonly DateTimeOffset now = DateTimeOffset.UtcNow;

    protected static readonly TimeSpan retry_timeout = TimeSpan.FromSeconds(5);
    protected static readonly DateTimeOffset after_timeout = now.Add(retry_timeout);

    protected static readonly ProcessingPosition CurrentProcessingPosition = new(22, 42);
    protected static readonly ProcessingPosition NextProcessingPosition = CurrentProcessingPosition.IncrementWithStream();

    protected static readonly StreamEvent Evt = new(
        committed_events.single(CurrentProcessingPosition.EventLogPosition),
        CurrentProcessingPosition.StreamPosition,
        StreamId.EventLog,
        partition,
        true);

    protected static readonly StreamProcessorState before_state = new StreamProcessorState(CurrentProcessingPosition, last_successful_processing);
}