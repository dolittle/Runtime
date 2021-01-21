// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_KnownClients
{
    public class when_asking_if_it_has_an_unknown_client : given.no_known_clients
    {
        static bool result;

        Because of = () => result = known_clients.HasFor(typeof(MyClient));

        It should_not_have_it = () => result.ShouldBeFalse();
    }
}