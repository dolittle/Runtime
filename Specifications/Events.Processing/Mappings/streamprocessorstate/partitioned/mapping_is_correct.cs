// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store.Actors;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Mappings;

public class mapping_a_partitioned_state : given.a_failing_partitioned_streamprocessorstate
{
    static StreamProcessorState before;
    static Bucket as_protobuf;
    static StreamProcessorState after;


    private Establish context = () =>
    {
        before = stream_processor_state;
        as_protobuf = stream_processor_state.ToProtobuf();
        after = (StreamProcessorState)as_protobuf.FromProtobuf();
    };
    
    It should_not_be_lossy = () => after.Should().BeEquivalentTo(before);
    
    It should_have_the_correct_type = () => after.ShouldBeOfExactType(before.GetType());
    
}