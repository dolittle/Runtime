// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_KnownClients
{
    public class when_asking_for_type_that_is_not_a_clientbase : given.no_known_clients
    {
        static Exception result;

        Because of = () => result = Catch.Exception(() => known_clients.GetFor(typeof(string)));

        It should_throw_type_does_not_implement_client_base = () => result.ShouldBeOfExactType<TypeDoesNotImplementClientBase>();
    }
}