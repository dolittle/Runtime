/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Applications.for_Clients
{
    public class when_disconnecting_first_of_two_clients : given.two_clients
    {
        static Clients clients;
        Establish context = () =>
        {
            clients = new Clients(Moq.Mock.Of<ILogger>());
            clients.Connect(first_client);
            clients.Connect(second_client);
        };

        Because of = () => clients.Disconnect(first_client.ClientId);

        It should_consider_client_disconnected = () => clients.IsConnected(first_client.ClientId);

        It should_only_have_second_client_as_connected_clients = () => clients.GetConnectedClients().ShouldContainOnly(new[] { second_client });
    }
}