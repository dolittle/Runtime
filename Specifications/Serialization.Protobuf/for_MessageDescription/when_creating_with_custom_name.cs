// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescription;

public class when_creating_with_custom_name
{
    const string custom_name = "CustomName";
    static MessageDescription result;

    Because of = () => result = new MessageDescription(typeof(class_with_properties), Array.Empty<PropertyDescription>(), custom_name);

    It should_set_custom_name = () => result.Name.ShouldEqual(custom_name);
}