// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_KnownClients;

public class when_asking_for_two_client_types_from_different_providers : given.two_known_clients_from_two_different_providers
{
    static Client first_result;
    static Client second_result;

    Because of = () =>
    {
        first_result = known_clients.GetFor(typeof(MyClient));
        second_result = known_clients.GetFor(typeof(MySecondClient));
    };

    It should_return_the_correct_first_client = () => first_result.Type.ShouldEqual(typeof(MyClient));
    It should_return_the_correct_second_client = () => second_result.Type.ShouldEqual(typeof(MySecondClient));
}