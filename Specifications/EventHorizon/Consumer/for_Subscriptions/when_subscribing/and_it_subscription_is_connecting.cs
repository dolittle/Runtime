// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Machine.Specifications;
using Microservices;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscriptions.when_subscribing;

public class and_it_subscription_is_connecting : given.all_dependencies
{

    Establish context = () =>
    {
        subscription
            .Setup(_ => _.Start())
            .Callback(() => subscription.SetupGet(_ => _.State).Returns(SubscriptionState.Connecting));

        subscriptions.Subscribe(subscription_id, execution_context);
        Task.Delay(50).GetAwaiter().GetResult();
    };

    static Task<SubscriptionResponse> result;
    Because of = () =>
    {
        result = subscriptions.Subscribe(subscription_id, execution_context);
        Task.Delay(50).GetAwaiter().GetResult();
    };

    It should_return_the_correct_task = () => result.ShouldEqual(connection_response_completion_source.Task);
    It should_not_create_a_new_subscription = () => subscription_factory.Verify(_ => _.Create(
        Moq.It.IsAny<SubscriptionId>(),
        Moq.It.IsAny<MicroserviceAddress>(),
        execution_context), Moq.Times.Once);

    It should_not_start_subscription_more_than_once = () => subscription.Verify(_ => _.Start(), Moq.Times.Once);

    Cleanup clean = () => connection_response_completion_source.SetCanceled();
}