// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Dynamic.Specs.for_WriteOnceExpandoObject
{
    [Subject(typeof(WriteOnceExpandoObject))]
    public class when_creating_and_setting_value_during_creation
    {
        static dynamic expando_object;

        Because of = () => expando_object = new WriteOnceExpandoObject(p => p.Something = "Hello world");

        It should_have_the_value_set = () => ((string)expando_object.Something).ShouldEqual("Hello world");
    }
}
