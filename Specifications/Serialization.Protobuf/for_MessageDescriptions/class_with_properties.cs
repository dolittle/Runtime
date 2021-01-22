// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescriptions
{
    public class class_with_properties
    {
        public static PropertyInfo first_property = typeof(class_with_properties).GetProperty("FirstProperty");
        public static PropertyInfo second_property = typeof(class_with_properties).GetProperty("SecondProperty");

        public int FirstProperty { get; set; }

        public string SecondProperty { get; set; }
    }
}