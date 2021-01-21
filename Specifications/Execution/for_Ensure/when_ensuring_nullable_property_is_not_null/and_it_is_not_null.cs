// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Execution.for_Ensure.when_ensuring_nullable_property_is_not_null
{
    [Subject(typeof(Ensure))]
    public class and_it_is_not_null
    {
        const string property_name = "test";
        static int? property;
        static Exception exception;

        Establish context = () => property = 5;
        Because of = () => exception = Catch.Exception(() => Ensure.IsNotNull(property_name, property));

        It should_not_throw_an_exception = () => exception.ShouldBeNull();
    }
}