// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ClientManager.when_getting.with_host_and_address;

public class and_type_does_not_implement_clientbase : given.a_client_manager
{
    static string host;
    static int port;
    static Exception result;

    Establish context = () =>
    {
        host = "host";
        port = 1;
    };

    Because of = () => result = Catch.Exception(() => client_manager.Get(typeof(string), host, port));

    It should_throw_type_does_not_implement_client_base = () => result.ShouldBeOfExactType<TypeDoesNotImplementClientBase>();
}