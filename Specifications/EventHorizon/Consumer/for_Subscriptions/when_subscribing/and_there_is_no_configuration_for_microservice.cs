// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscriptions.when_subscribing;

public class and_there_is_no_configuration_for_microservice : given.all_dependencies
{
    static SubscriptionResponse result;
    Because of = () => result = subscriptions.Subscribe(subscription_id with
    {
        ProducerMicroserviceId = Guid.Parse("565fafa6-d0ef-4662-96a3-b291abffb0fc")
    }, execution_context).GetAwaiter().GetResult();

    It should_return_failure_response = () => result.Success.Should().BeFalse();
    It should_have_correct_failure_id = () => result.Failure.Id.Should().Be(SubscriptionFailures.MissingMicroserviceConfiguration);
}