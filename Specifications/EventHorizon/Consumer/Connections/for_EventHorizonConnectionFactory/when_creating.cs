// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Dolittle.Runtime.Services.Clients;
using Dolittle.Runtime.EventHorizon.Contracts;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections.for_EventHorizonConnectionFactory;

public class when_creating : given.all_dependencies
{
    Establish context = () =>
    {
        reverse_call_clients
            .Setup(_ => _.GetFor(
                Moq.It.IsAny<EventHorizonProtocol>(),
                microservice_address.Host,
                microservice_address.Port,
                Moq.It.IsAny<TimeSpan>()))
            .Returns(Moq.Mock.Of<IReverseCallClient<ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse>>());
    };
    static IEventHorizonConnection result;
    Because of = () => result = factory.Create(microservice_address, execution_contexts.create());
    It should_return_the_connection = () => result.ShouldNotBeNull();
    It should_create_the_reverse_call_client = () => reverse_call_clients.Verify(_ => _.GetFor(
        Moq.It.IsAny<EventHorizonProtocol>(),
        microservice_address.Host,
        microservice_address.Port,
        Moq.It.IsAny<TimeSpan>()), Moq.Times.Once);
}