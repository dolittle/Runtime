// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Threading;
using Dolittle.Protobuf;
using Grpc.Core;
using Machine.Specifications;
using grpc = contracts::Dolittle.Runtime.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_ConsumerClient.when_acknowledging_consent
{
    public class and_consent_is_not_acknowledged : given.all_dependencies
    {
        static Exception result;

        Establish context = () => grpc_consumer_client.Setup(_ =>
            _.AcknowledgeConsent(Moq.It.IsAny<grpc.AcknowledgeRequest>(), Moq.It.IsAny<Metadata>(), Moq.It.IsAny<DateTime?>(), Moq.It.IsAny<CancellationToken>())).Returns(new grpc.AcknowledgeResponse { Acknowledged = false });

        Because of = () => result = Catch.Exception(() => consumer_client.AcknowledgeConsent(event_horizon, microservice_address));

        It should_get_correct_client = () => client_manager.Verify(_ => _.Get<grpc.Consumer.ConsumerClient>(microservice_address.Host, microservice_address.Port), Moq.Times.Once);

        It should_acknowledge_consent_for_correct_event_horizon = () => grpc_consumer_client
            .Verify(
                _ => _.AcknowledgeConsent(
                    new grpc.AcknowledgeRequest { Microservice = event_horizon.ProducerMicroservice.ToProtobuf(), Tenant = event_horizon.ProducerTenant.ToProtobuf() },
                    Moq.It.IsAny<Metadata>(),
                    Moq.It.IsAny<DateTime?>(),
                    Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);

        It should_fail_because_there_is_no_consent = () => result.ShouldBeOfExactType<NoConsentForEventHorizon>();
    }
}