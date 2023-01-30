// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Processing.Mappings;

public class mapping_a_partitioned_state : given.a_failing_partitioned_streamprocessorstate
{
    static IStreamProcessorState before;
    static Bucket as_protobuf;
    static IStreamProcessorState after;


    private Establish context = () =>
    {
        before = stream_processor_state;
        as_protobuf = stream_processor_state.ToProtobuf();
        after = as_protobuf.FromProtobuf();
    };
    
    It should_not_be_lossy = () => after.Should().BeEquivalentTo(before);
    
    It should_have_the_correct_type = () => after.ShouldBeOfExactType(before.GetType());
    
}