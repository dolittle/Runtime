// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Machine.Specifications;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Dolittle.Runtime.Services.Clients;
using Microservices;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections.for_EventHorizonConnectionFactory.given;

public class all_dependencies
{
    protected static Mock<IReverseCallClients> reverse_call_clients;
    protected static MicroserviceAddress microservice_address;
    protected static EventHorizonConnectionFactory factory;

    Establish context = () =>
    {
        reverse_call_clients = new Mock<IReverseCallClients>();
        microservice_address = new MicroserviceAddress("host", 42);

        factory = new EventHorizonConnectionFactory(
            reverse_call_clients.Object,
            Mock.Of<IMetricsCollector>(),
            NullLoggerFactory.Instance);
    };
}