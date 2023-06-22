// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Integration.Tests.Events.Processing.StreamProcessorStates.SubscriptionId;

class with_no_events : given.a_clean_event_store
{
    static TenantId consumer_tenant_id;
    static MicroserviceId producer_microservice_id;
    static TenantId producer_tenant_id;
    static ScopeId scope_id;
    static StreamId stream_id;
    static PartitionId partition_id;


    static Dolittle.Runtime.EventHorizon.Consumer.SubscriptionId subscription_id;

    static Try<IStreamProcessorState> result;

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

        result = stream_processor_states.TryGetFor(subscription_id, CancellationToken.None).GetAwaiter().GetResult();
    };


    It should_return_a_failure = () => result.Success.Should().BeFalse();
    
    It should_return_a_stream_processor_state_not_found_exception = () => result.Exception.Should().BeOfType<StreamProcessorStateDoesNotExist>();
}