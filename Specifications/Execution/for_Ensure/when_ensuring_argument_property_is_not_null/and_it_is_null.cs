// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Execution.for_Ensure.when_ensuring_argument_property_is_not_null;

[Subject(typeof(Ensure))]
public class and_it_is_null
{
    const string path = "my_object";
    const string property_name = "test";
    static Exception exception;
    Because of = () => exception = Catch.Exception(() => Ensure.ArgumentPropertyIsNotNull<string>(property_name, path, null));

    It should_throw_an_exception = () => exception.ShouldNotBeNull();
    It should_be_an_argument_null_exception = () => exception.ShouldBeOfExactType<NotEnsured>();
    It should_include_the_path_and_property_name_in_the_exception = () => ((NotEnsured)exception)?.Property.ShouldEqual($"{path}.{property_name}");
}