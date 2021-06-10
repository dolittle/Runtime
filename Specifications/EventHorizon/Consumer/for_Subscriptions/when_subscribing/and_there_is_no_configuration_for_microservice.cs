// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Machine.Specifications;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscriptions.when_subscribing
{
    public class and_there_is_no_configuration_for_microservice : given.all_dependencies
    {
        static SubscriptionResponse result;
        static SubscriptionId unknown_producer_subscription;
        
        Establish context = () => unknown_producer_subscription = new(
                    "aa6403de-6dee-4db8-80a0-366bb9532b3a",
                    // unknown producer microservice id
                    Guid.Parse("565fafa6-d0ef-4662-96a3-b291abffb0fc"),
                    "1da8200b-6da6-44fc-a442-29210fb5b934",
                    "51355173-6c09-4efa-bd4f-122c6cbc67fc",
                    Guid.Parse("cd2433d3-4aad-41ac-87bc-f37240736444"),
                    "064749fc-8d13-4b12-a3c4-584e34596b99");

        Because of = () => result = subscriptions.Subscribe(unknown_producer_subscription).GetAwaiter().GetResult();

        It should_return_failure_response = () => result.Success.ShouldBeFalse();
        It should_have_correct_failure_id = () => result.Failure.Id.ShouldEqual(SubscriptionFailures.MissingMicroserviceConfiguration);
    }
}
