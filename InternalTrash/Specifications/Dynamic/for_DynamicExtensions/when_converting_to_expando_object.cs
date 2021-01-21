// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Dynamic;
using Machine.Specifications;

namespace Dolittle.Specs.Dynamic.for_DynamicExtensions
{
    public class when_converting_to_expando_object
    {
        static ObjectWithProperties source;
        static dynamic result;

        Establish context = () => source = new ObjectWithProperties { SomeInt = 42, SomeDouble = 42.42, SomeString = "FourtyTwo" };

        Because of = () => result = source.AsExpandoObject();

        It should_include_the_integer = () => ShouldExtensionMethods.ShouldEqual(result.SomeInt, source.SomeInt);
        It should_include_the_double = () => ShouldExtensionMethods.ShouldEqual(result.SomeDouble, source.SomeDouble);
        It should_include_the_string = () => ShouldExtensionMethods.ShouldEqual(result.SomeString, source.SomeString);
    }
}
