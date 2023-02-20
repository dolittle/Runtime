// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;

namespace Events.Processing.Tests;

public class StreamProcessorStateTests
{
    private const string Partition = "partition";
    readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    static readonly StreamEvent Evt = new(
        committed_events.single(EventLogSequenceNumber.Initial),
        StreamPosition.Start,
        StreamId.EventLog,
        Partition,
        true);

    [Fact]
    public void ShouldIncrementStreamPositionOnSuccessFulProcessing()
    {
        var before = StreamProcessorState.New;
        var afterState = before.WithSuccessfullyProcessed(Evt, Now);

        afterState.Should().BeEquivalentTo(new StreamProcessorState(ProcessingPosition.Initial.IncrementWithStream(),
            ImmutableDictionary<PartitionId, FailingPartitionState>.Empty, Now));
    }
}