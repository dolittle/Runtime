// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using System.Threading;
using ReverseCallClient = Dolittle.Runtime.Services.Clients.IReverseCallClient<
                            Dolittle.Runtime.EventHorizon.Contracts.ConsumerSubscriptionRequest,
                            Dolittle.Runtime.EventHorizon.Contracts.SubscriptionResponse,
                            Dolittle.Runtime.EventHorizon.Contracts.ConsumerRequest,
                            Dolittle.Runtime.EventHorizon.Contracts.ConsumerResponse>;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections.for_EventHorizonConnection.given
{
    public class all_dependencies
    {
        protected static Mock<ReverseCallClient> reverse_call_client;
        protected static SubscriptionId subscription;
        protected static CancellationToken cancellation_token;
        protected static EventHorizonConnection connection;

        Establish context = () =>
        {
            reverse_call_client = new Mock<ReverseCallClient>();
            subscription = new SubscriptionId(
                "aa6403de-6dee-4db8-80a0-366bb9532b3a",
                Guid.Parse("8aef7b88-db5b-4e17-92f7-cfd1cb23a829"),
                "1da8200b-6da6-44fc-a442-29210fb5b934",
                "51355173-6c09-4efa-bd4f-122c6cbc67fc",
                Guid.Parse("cd2433d3-4aad-41ac-87bc-f37240736444"),
                "064749fc-8d13-4b12-a3c4-584e34596b99"
            );

            cancellation_token = CancellationToken.None;

            connection = new EventHorizonConnection(
                reverse_call_client.Object,
                Mock.Of<IMetricsCollector>(),
                Mock.Of<ILogger>());
        };
    }
}
