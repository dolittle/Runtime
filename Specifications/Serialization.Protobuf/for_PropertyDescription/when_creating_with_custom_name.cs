// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_PropertyDescription;

public class when_creating_with_custom_name
{
    const string custom_name = "SomeName";
    static PropertyDescription result;

    Because of = () => result = new PropertyDescription(class_with_property.some_property, custom_name);
    It should_set_name_to_custom_name = () => result.Name.ShouldEqual(custom_name);
}