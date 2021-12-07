// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Grpc.Core;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ClientManager.when_getting;

public class and_type_does_not_have_a_constructor_that_takes_callinvoker : given.a_client_manager
{
    class MyClient : ClientBase
    {
    }

    static string host;
    static int port;
    static Exception result;

    Establish context = () =>
    {
        host = "host";
        port = 1;
    };

    Because of = () => result = Catch.Exception(() => client_manager.Get(typeof(MyClient), host, port));

    It should_throw_missing_expected_constructor_for_client_type = () => result.ShouldBeOfExactType<MissingExpectedConstructorForClientType>();
}