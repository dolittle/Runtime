// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_PropertyDescription;

public class when_creating_with_default_value
{
    const string default_value = "The default value";
    static PropertyDescription result;

    Because of = () => result = new PropertyDescription(class_with_property.some_property, defaultValue: default_value);

    It should_set_default_value = () => result.DefaultValue.ShouldEqual(default_value);
}