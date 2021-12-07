// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_KnownClients;

public class when_asking_for_unknown_client : given.no_known_clients
{
    static Exception result;

    Because of = () => result = Catch.Exception(() => known_clients.GetFor(typeof(MyClient)));

    It should_throw_unknown_client_type = () => result.ShouldBeOfExactType<UnknownClientType>();
}