// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Machine.Specifications;
using Dolittle.Runtime.EventHorizon.Contracts;
using FluentAssertions;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections.for_EventHorizonConnection.when_connecting;

public class and_connection_failed : given.all_dependencies
{
    Establish context = () =>
    {
        reverse_call_client
            .Setup(_ => _.Connect(Moq.It.IsAny<ConsumerSubscriptionRequest>(), execution_context, cancellation_token))
            .Returns(Task.FromResult(false));
    };
    static SubscriptionResponse result;
    Because of = () => result = connection.Connect(subscription, 0, cancellation_token).GetAwaiter().GetResult();
    It should_return_failed_response = () => result.Success.Should().BeFalse();
    It should_return_failed_response_with_correct_failure_id = () => result.Failure.Id.Should().Be(SubscriptionFailures.CouldNotConnectToProducerRuntime);
    It should_call_connect_on_reverse_call_client = () => reverse_call_client.Verify(_ => _.Connect(
        Moq.It.IsAny<ConsumerSubscriptionRequest>(),
        execution_context,
        cancellation_token), Moq.Times.Once);
}