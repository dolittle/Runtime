// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_PropertyDescription
{
    public class when_creating_without_custom_name
    {
        static PropertyDescription result;

        Because of = () => result = new PropertyDescription(class_with_property.some_property);

        It should_hold_the_name_of_the_property = () => result.Name.ShouldEqual(class_with_property.some_property.Name);
    }
}