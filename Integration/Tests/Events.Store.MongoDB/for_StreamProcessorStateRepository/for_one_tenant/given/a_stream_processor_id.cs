// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams;
using Machine.Specifications;

namespace Integration.Tests.Events.Store.MongoDB.for_StreamProcessorStateRepository.for_one_tenant.given;

public class a_stream_processor_id : MongoDB.given.a_clean_event_store
{
    protected static StreamProcessorId stream_processor_id;

    Establish context = () =>
    {
        stream_processor_id = new StreamProcessorId("b7aa9a19-1f34-42ad-9e65-cd01a0239cb0", "bc15dd0b-1724-4716-9226-f0b722d22206", "b61ed8ba-4e3e-4369-a088-9f60d72777aa");
    };

    protected static StreamProcessorState an_unpartitioned_state() => StreamProcessorState.New;
    protected static Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState a_partitioned_state() => Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState.New;
    
}