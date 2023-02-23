// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;

namespace Events.Processing.Tests.NonPartitioned.given;

public class NonFailingState
{
    protected const string Partition = "partition";
    protected static readonly DateTimeOffset before = DateTimeOffset.UtcNow;
    protected static readonly DateTimeOffset now = DateTimeOffset.UtcNow;

    protected static readonly ProcessingPosition CurrentProcessingPosition = new(22, 42);

    protected static readonly StreamEvent Evt = new(
        committed_events.single(CurrentProcessingPosition.EventLogPosition),
        CurrentProcessingPosition.StreamPosition,
        StreamId.EventLog,
        Partition,
        false);

    protected static readonly StreamProcessorState before_state = new(CurrentProcessingPosition, before);
}