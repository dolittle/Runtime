// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescription
{
    public class when_creating_with_properties
    {
        static MessageDescription result;

        Because of = () => result = new MessageDescription(typeof(class_with_properties), new[]
                                        {
                                            new PropertyDescription(class_with_properties.first_property),
                                            new PropertyDescription(class_with_properties.second_property)
                                        });

        It should_hold_first_property = () => result.Properties.ToArray()[0].Property.ShouldEqual(class_with_properties.first_property);
        It should_hold_second_property = () => result.Properties.ToArray()[1].Property.ShouldEqual(class_with_properties.second_property);
    }
}