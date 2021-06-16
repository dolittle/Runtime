// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Grpc.Core;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_CallInvokerManager.when_getting_for_type
{
    public class and_type_is_a_public_known_service : given.a_call_invoker_manager
    {
        static Client client;
        static CallInvoker call_invoker;

        Establish context = () =>
        {
            client = new Client(EndpointVisibility.Public, typeof(MyClient), null);
            known_clients.Setup(_ => _.GetFor(typeof(MyClient))).Returns(client);
        };

        Because of = () => call_invoker = manager.GetFor(typeof(MyClient));

        It should_return_an_invoker = () => call_invoker.ShouldNotBeNull();
    }
}