// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;

namespace Integration.Tests.Events.Processing.StreamProcessorStates.SubscriptionId.non_default_scope;

class nonpartitioned : given.a_clean_event_store
{
    static readonly DateTimeOffset last_successfully_processed = DateTimeOffset.Now;

    static TenantId consumer_tenant_id;
    static MicroserviceId producer_microservice_id;
    static TenantId producer_tenant_id;
    static ScopeId scope_id;
    static StreamId stream_id;
    static PartitionId partition_id;


    static Dolittle.Runtime.EventHorizon.Consumer.SubscriptionId subscription_id;
    static StreamProcessorState stream_processor_state;
    static StreamProcessorState retrieved_stream_processor_state;


    Establish context = () =>
    {
        consumer_tenant_id = Guid.NewGuid();
        producer_microservice_id = Guid.NewGuid();
        producer_tenant_id = tenant;
        scope_id = Guid.NewGuid();
        stream_id = Guid.NewGuid();
        partition_id = "partition";

        subscription_id = new(consumer_tenant_id,
            producer_microservice_id,
            producer_tenant_id,
            scope_id,
            stream_id,
            partition_id);

        stream_processor_state = new(new StreamPosition(10), 15, "", last_successfully_processed, 0, last_successfully_processed, false);

        stream_processor_states.Persist(subscription_id, stream_processor_state, CancellationToken.None).GetAwaiter().GetResult();
        retrieved_stream_processor_state_response = stream_processor_states.TryGetFor(subscription_id, CancellationToken.None).GetAwaiter().GetResult();

        retrieved_stream_processor_state = (StreamProcessorState)retrieved_stream_processor_state_response.Result;
    };

    It should_have_retrieved_the_state_successfully = () => retrieved_stream_processor_state_response.Success.Should().BeTrue();


    It should_have_the_correct_stream_processor_state_type = () =>
        retrieved_stream_processor_state_response.Result.Should().BeOfType<StreamProcessorState>();

    It should_have_the_correct_stream_processor_state = () => retrieved_stream_processor_state.Should().BeEquivalentTo(stream_processor_state);
    static Try<IStreamProcessorState> retrieved_stream_processor_state_response;
}