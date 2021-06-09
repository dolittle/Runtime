// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Microservices;
using System.Collections.Generic;
using Dolittle.Runtime.ApplicationModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.EventHorizon.Consumer.Connections;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscription.given
{
    public class all_dependencies
    {
        protected static SubscriptionId subscription_id;
        protected static MicroserviceAddress producer_microservice_address;
        protected static Mock<IEventHorizonConnectionFactory> event_horizon_connection_factory;
        protected static Mock<IStreamProcessorFactory> stream_processor_factory;
        protected static Mock<IGetNextEventToReceiveForSubscription> get_next_event;
        protected static Mock<IAsyncPolicyFor<Subscription>> policy;
        protected static Subscription subscription;

        Establish context = () =>
        {
            subscription_id = new SubscriptionId(
                "aa6403de-6dee-4db8-80a0-366bb9532b3a",
                Guid.Parse("8aef7b88-db5b-4e17-92f7-cfd1cb23a829"),
                "1da8200b-6da6-44fc-a442-29210fb5b934",
                "51355173-6c09-4efa-bd4f-122c6cbc67fc",
                Guid.Parse("cd2433d3-4aad-41ac-87bc-f37240736444"),
                "064749fc-8d13-4b12-a3c4-584e34596b99"
            );
            producer_microservice_address = new MicroserviceAddress("host", 42);
            event_horizon_connection_factory = new Mock<IEventHorizonConnectionFactory>();
            stream_processor_factory = new Mock<IStreamProcessorFactory>();
            get_next_event = new Mock<IGetNextEventToReceiveForSubscription>();
            policy = new Mock<IAsyncPolicyFor<Subscription>>();

            subscription = new Subscription(
                subscription_id,
                producer_microservice_address,
                policy.Object,
                event_horizon_connection_factory.Object,
                stream_processor_factory.Object,
                get_next_event.Object,
                Mock.Of<ILogger>());
        };
    }
}
