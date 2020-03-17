// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Streams.for_StreamProcessorState
{
    public class when_creating_state
    {
        static StreamPosition stream_position;
        static IDictionary<PartitionId, FailingPartitionState> failing_partitions;
        static StreamProcessorState state;

        Establish context = () =>
        {
            stream_position = 0;
            failing_partitions = new Dictionary<PartitionId, FailingPartitionState>();
        };

        Because of = () => state = new StreamProcessorState(stream_position, failing_partitions);

        It should_have_the_correct_stream_position = () => state.Position.ShouldEqual(stream_position);
    }
}