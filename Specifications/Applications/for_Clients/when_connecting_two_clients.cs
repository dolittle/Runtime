/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Applications.for_Clients
{
    public class when_connecting_two_clients : given.two_clients
    {
        static Clients clients;

        Establish context = () => clients = new Clients(Moq.Mock.Of<ILogger>());

        Because of = () =>
        {
            clients.Connect(first_client);
            clients.Connect(second_client);
        };

        It should_have_both_clients_as_connected_clients = () => clients.GetConnectedClients().ShouldContainOnly(new [] { first_client, second_client });
    }
}