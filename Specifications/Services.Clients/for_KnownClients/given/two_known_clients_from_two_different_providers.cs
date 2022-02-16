// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Services.Clients.for_KnownClients.given;

public class two_known_clients_from_two_different_providers
{
    protected static KnownClients known_clients;
    protected static IEnumerable<IKnowAboutClients> providers;

    protected static Mock<IKnowAboutClients> first_provider;
    protected static Mock<IKnowAboutClients> second_provider;

    protected static Client first_client;
    protected static Client second_client;

    Establish context = () =>
    {
        first_client = new Client(EndpointVisibility.Public, typeof(MyClient), null);
        first_provider = new Mock<IKnowAboutClients>();
        first_provider.Setup(_ => _.Clients).Returns(new[] { first_client });

        second_client = new Client(EndpointVisibility.Public, typeof(MySecondClient), null);
        second_provider = new Mock<IKnowAboutClients>();
        second_provider.Setup(_ => _.Clients).Returns(new[] { second_client });

        providers = new []{first_provider.Object, second_provider.Object};
        known_clients = new KnownClients(providers);
    };
}