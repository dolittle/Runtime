// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Dolittle.Runtime.Serialization.Protobuf.for_PropertyDescription;

public class class_with_property
{
    public static PropertyInfo some_property = typeof(class_with_property).GetProperty("SomeProperty");

    public int SomeProperty { get; set; }
}