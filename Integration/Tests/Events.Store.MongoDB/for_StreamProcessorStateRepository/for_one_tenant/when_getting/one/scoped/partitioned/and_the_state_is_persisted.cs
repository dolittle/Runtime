// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Processing.Streams.Partitioned;
using Dolittle.Runtime.Events.Store.Streams;
using Integration.Tests.Events.Store.MongoDB.for_StreamProcessorStateRepository.for_one_tenant.given;
using Machine.Specifications;

namespace Integration.Tests.Events.Store.MongoDB.for_StreamProcessorStateRepository.for_one_tenant.when_getting.one.scoped.partitioned;

public class and_the_state_is_persisted : given.all_dependencies
{
    static (StreamProcessorId id, IStreamProcessorState state) another_stored_processor;

    Establish context = () =>
    {
        stream_processor_id = stream_processor_id with
        {
            ScopeId = "c424eedc-dc60-4c34-9860-536e206fa9e8"
        };

        var failing_partitions = new Dictionary<PartitionId, FailingPartitionState>();
        failing_partitions.Add("d2b8f91d-e838-4b34-b0a3-0468dc29ca0e",
            new FailingPartitionState(2, 5, DateTimeOffset.UtcNow, "Some reason", 2, DateTimeOffset.UtcNow));

        stored_state = a_partitioned_state() with
        {
            Position = 2,
            EventLogPosition = 5,
            FailingPartitions = failing_partitions
        };
        another_stored_processor = (stream_processor_id with
        {
            EventProcessorId = "9ec32ae1-ed08-4781-8fc2-96ac7017aedd"
        }, a_partitioned_state());

        var to_store = new Dictionary<StreamProcessorId, IStreamProcessorState>();
        to_store.Add(stream_processor_id, stored_state);
        to_store.Add(another_stored_processor.id, another_stored_processor.state);

        stream_processor_state_repository.PersistForScope(another_stored_processor.id.ScopeId, to_store, CancellationToken.None).GetAwaiter().GetResult();
    };

    Because of = get_stream_processor_state;

    It should_get_the_state = () => retrieved_state.Success.ShouldBeTrue();
    It should_get_unpartitioned_state = () => retrieved_state.Result.Partitioned.ShouldBeTrue();

    It should_get_unpartitioned_state_type = () =>
        retrieved_state.Result.ShouldBeOfExactType<Dolittle.Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState>();

    It should_get_the_correct_state = () => retrieved_state.Result.should_be_the_same_partitioned_state_as(stored_state);
}