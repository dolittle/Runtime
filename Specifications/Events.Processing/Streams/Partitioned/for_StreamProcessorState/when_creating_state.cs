// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_StreamProcessorState;

public class when_creating_state
{
    static ProcessingPosition stream_position;
    static ImmutableDictionary<PartitionId, FailingPartitionState> failing_partitions;
    static StreamProcessorState state;
    static DateTimeOffset last_successfully_processed;

    Establish context = () =>
    {
        stream_position = ProcessingPosition.Initial;
        failing_partitions = ImmutableDictionary<PartitionId, FailingPartitionState>.Empty;
        last_successfully_processed = DateTimeOffset.UtcNow;
    };

    Because of = () => state = new StreamProcessorState(stream_position, failing_partitions, last_successfully_processed);

    It should_have_the_correct_stream_position = () => state.Position.ShouldEqual(stream_position);
    It should_have_the_correct_failing_partitions = () => state.FailingPartitions.ShouldEqual(failing_partitions);
    It should_be_partitioned = () => state.Partitioned.ShouldBeTrue();
    It should_have_the_correct_last_successfuly_processed_value = () => state.LastSuccessfullyProcessed.ShouldEqual(last_successfully_processed);
}