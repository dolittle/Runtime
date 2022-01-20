// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_ClientManager.when_getting;

public class and_type_does_not_implement_clientbase : given.a_client_manager
{
    static Exception result;

    Because of = () => result = Catch.Exception(() => client_manager.Get(typeof(string)));

    It should_throw_type_does_not_implement_client_base = () => result.ShouldBeOfExactType<TypeDoesNotImplementClientBase>();
}