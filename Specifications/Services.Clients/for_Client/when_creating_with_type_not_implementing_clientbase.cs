// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients;

public class when_creating_with_type_not_implementing_clientbase
{
    static Exception result;

    Because of = () => result = Catch.Exception(() => new Client(EndpointVisibility.Public, typeof(string), null));

    It should_throw_type_does_not_implement_client_base = () => result.ShouldBeOfExactType<TypeDoesNotImplementClientBase>();
}