// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.Clients.for_CallInvokerManager.when_getting_for_type
{
    public class and_type_is_not_client_base : given.a_call_invoker_manager
    {
        static Exception result;

        Because of = () => result = Catch.Exception(() => manager.GetFor(typeof(string)));

        It should_throw_type_does_not_implement_client_base = () => result.ShouldBeOfExactType<TypeDoesNotImplementClientBase>();
    }
}