using System.Threading.Tasks;
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using System.Threading;
using Dolittle.Runtime.Services.Clients;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections.for_EventHorizonConnection.when_connecting
{
    public class and_producer_responds_with_failure : given.all_dependencies
    {
        static Dolittle.Protobuf.Contracts.Failure failure;
        Establish context = () =>
        {
            failure = new Dolittle.Protobuf.Contracts.Failure
            {
                Id = Guid.Parse("79d0c188-8c0d-4498-90c5-3de5f2d479a0").ToProtobuf(),
                Reason = "some reason"
            };
            reverse_call_client
                .Setup(_ => _.Connect(Moq.It.IsAny<ConsumerSubscriptionRequest>(), cancellation_token))
                .Returns(Task.FromResult(true));
            reverse_call_client
                .SetupGet(_ => _.ConnectResponse)
                .Returns(new Contracts.SubscriptionResponse
                {
                    Failure = failure
                });
        };

        static SubscriptionResponse result;

        Because of = () => result = connection.Connect(subscription, 0, cancellation_token).GetAwaiter().GetResult();
        It should_return_failed_response = () => result.Success.ShouldBeFalse();
        It should_return_failed_response_with_correct_failure_id = () => result.Failure.Id.Value.ShouldEqual(failure.Id.ToGuid());
        It should_return_failed_response_with_correct_failure_reason = () => result.Failure.Reason.Value.ShouldEqual(failure.Reason);
        It should_call_connect_on_reverse_call_client = () => reverse_call_client.Verify(_ => _.Connect(
                                                            Moq.It.IsAny<ConsumerSubscriptionRequest>(),
                                                            cancellation_token), Moq.Times.Once);
    }
}