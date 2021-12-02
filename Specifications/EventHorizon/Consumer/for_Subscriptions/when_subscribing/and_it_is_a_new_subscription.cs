// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Microservices;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscriptions.when_subscribing;

public class and_it_is_a_new_subscription : given.all_dependencies
{
    static Task<SubscriptionResponse> result;
    Because of = () =>
    {
        result = subscriptions.Subscribe(subscription_id);
        Task.Delay(50).GetAwaiter().GetResult();
    };

    It should_return_the_correct_task = () => result.ShouldEqual(connection_response_completion_source.Task);
    It should_create_the_subscription = () => subscription_factory.Verify(_ => _.Create(
        subscription_id,
        configured_microservice.Address), Moq.Times.Once);

    It should_start_subscription_once = () => subscription.Verify(_ => _.Start(), Moq.Times.Once);

    Cleanup clean = () => connection_response_completion_source.SetCanceled();
}