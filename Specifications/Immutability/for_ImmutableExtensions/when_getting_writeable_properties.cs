// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Machine.Specifications;

namespace Dolittle.Runtime.Immutability.for_ImmutableExtensions
{
    public class when_getting_writeable_properties
    {
        static PropertyInfo[] properties;

        Because of = () => properties = typeof(class_with_properties_with_setters).GetWriteableProperties();

        It should_return_the_writeable_property = () => properties[0].Name.ShouldEqual("property_with_setter");
    }
}