// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Integration.Tests.Events.Store.MongoDB.for_StreamProcessorStateRepository.for_one_tenant.when_getting.one.scoped.partitioned;

public class and_there_are_no_states : given.all_dependencies
{
    Establish context = () =>
    {
        stream_processor_id = stream_processor_id with
        {
            ScopeId = "c156f336-8e2d-496b-8324-0268bda3e339"
        };
    };

    Because of = get_stream_processor_state;

    It should_not_get_the_state = () => retrieved_state.Success.ShouldBeFalse();
    It should_fail_because_state_does_not_exist = () => retrieved_state.Exception.ShouldBeOfExactType<StreamProcessorStateDoesNotExist>();
}