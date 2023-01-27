// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Events.Store.EventHorizon;
using FluentAssertions;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections.for_EventHorizonConnection.when_connecting;

public class and_producer_responds_with_consent : given.all_dependencies
{
    static ConsentId consent;
    Establish context = () =>
    {
        consent = Guid.Parse("0bb478f4-b9f1-41e3-808a-aee0c74e5938");
        reverse_call_client
            .Setup(_ => _.Connect(Moq.It.IsAny<ConsumerSubscriptionRequest>(), execution_context, cancellation_token))
            .Returns(Task.FromResult(true));
        reverse_call_client
            .SetupGet(_ => _.ConnectResponse)
            .Returns(new Contracts.SubscriptionResponse
            {
                ConsentId = consent.ToProtobuf()
            });
    };

    static SubscriptionResponse result;

    Because of = () => result = connection.Connect(subscription, 0, cancellation_token).GetAwaiter().GetResult();
    It should_return_successful_response = () => result.Success.Should().BeTrue();
    It should_return_the_consent = () => result.ConsentId.Should().Be(consent);
    It should_call_connect_on_reverse_call_client = () => reverse_call_client.Verify(_ => _.Connect(
        Moq.It.IsAny<ConsumerSubscriptionRequest>(),
        execution_context, 
        cancellation_token), Moq.Times.Once);
}