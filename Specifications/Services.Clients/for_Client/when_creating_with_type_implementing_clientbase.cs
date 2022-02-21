// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Grpc.Core;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients;

public class when_creating_with_type_implementing_clientbase
{
    class MyClient : ClientBase { }

    static Client result;

    Because of = () => result = new Client(EndpointVisibility.Public, typeof(MyClient), null);

    It should_initialize_the_instance = () => result.ShouldNotBeNull();
}