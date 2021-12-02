// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescription;

public class when_creating_without_custom_name
{
    static MessageDescription result;

    Because of = () => result = new MessageDescription(typeof(class_with_properties), Array.Empty<PropertyDescription>());

    It should_set_name_to_type_name = () => result.Name.ShouldEqual(typeof(class_with_properties).Name);
}