// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescription;

public class simple_object_with_primitives
{
    public static PropertyInfo a_guid_property = typeof(simple_object_with_primitives).GetProperty("a_guid");
    public static PropertyInfo an_integer_property = typeof(simple_object_with_primitives).GetProperty("an_integer");
    public static PropertyInfo a_float_property = typeof(simple_object_with_primitives).GetProperty("a_float");
    public static PropertyInfo a_double_property = typeof(simple_object_with_primitives).GetProperty("a_double");
    public static PropertyInfo a_string_property = typeof(simple_object_with_primitives).GetProperty("a_string");
    public static PropertyInfo a_date_time_property = typeof(simple_object_with_primitives).GetProperty("a_date_time");
    public static PropertyInfo a_date_time_offset_property = typeof(simple_object_with_primitives).GetProperty("a_date_time_offset");

    public Guid a_guid { get; set; }

    public int an_integer { get; set; }

    public float a_float { get; set; }

    public double a_double { get; set; }

    public string a_string { get; set; }

    public DateTime a_date_time { get; set; }

    public DateTimeOffset a_date_time_offset { get; set; }
}