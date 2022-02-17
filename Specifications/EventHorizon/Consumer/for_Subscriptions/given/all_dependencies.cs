// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using System.Collections.Generic;
using Dolittle.Runtime.ApplicationModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Microservices;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscriptions.given;

public record MicroservicesEntry(MicroserviceId Id, MicroserviceAddress Address);
public class all_dependencies
{
    protected static TaskCompletionSource<SubscriptionResponse> connection_response_completion_source;
    protected static SubscriptionId subscription_id;
    protected static MicroservicesEntry configured_microservice;
    protected static Mock<ISubscriptionFactory> subscription_factory;
    protected static Mock<ISubscription> subscription;
    protected static MicroservicesConfiguration microservices_configuration;
    protected static Subscriptions subscriptions;

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
        configured_microservice = new MicroservicesEntry(subscription_id.ProducerMicroserviceId, new MicroserviceAddress("host", 3));

        subscription_factory = new Mock<ISubscriptionFactory>();
        subscription = new Mock<ISubscription>();
        subscription_factory
            .Setup(_ => _.Create(subscription_id, configured_microservice.Address))
            .Returns(subscription.Object);
        subscription.SetupGet(_ => _.State).Returns(SubscriptionState.Created);
        subscription
            .Setup(_ => _.Start())
            .Callback(() => subscription.SetupGet(_ => _.State).Returns(SubscriptionState.Connected));
        connection_response_completion_source = new TaskCompletionSource<SubscriptionResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

        subscription
            .SetupGet(_ => _.ConnectionResponse)
            .Returns(connection_response_completion_source.Task);
        microservices_configuration = new MicroservicesConfiguration(
            new Dictionary<Guid, MicroserviceAddressConfiguration>
            {
                {
                    configured_microservice.Id,
                    new MicroserviceAddressConfiguration(
                        configured_microservice.Address.Host,
                        configured_microservice.Address.Port)
                }
            });

        subscriptions = new Subscriptions(
            microservices_configuration,
            subscription_factory.Object,
            Mock.Of<IMetricsCollector>(),
            Mock.Of<ILogger>());
    };

    protected static IEnumerable<bool> ClearMicroservicesConfiguration()
        => microservices_configuration
            .Keys
            .Select(microservice => microservices_configuration.Remove(microservice, out var _));

    protected static void AddMicroservice(MicroserviceId microserviceId, string host, int port)
        => microservices_configuration.TryAdd(microserviceId, new MicroserviceAddressConfiguration(host, port));

}