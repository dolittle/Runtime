// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Serialization.Protobuf.for_Serializer;

public class when_serializing_simple_object_with_primitives : given.a_serializer
{
    static simple_object_with_primitives original;
    static simple_object_with_primitives deserialized;

    static byte[] serialized_bytes;

    Establish context = () =>
    {
        original = new simple_object_with_primitives
        {
            a_guid = Guid.NewGuid(),
            an_integer = 42,
            a_float = 43.44f,
            a_double = 45.46,
            a_string = "Fourty Two",
            a_date_time = DateTime.UtcNow,
            a_date_time_offset = DateTimeOffset.UtcNow
        };

        message_descriptions.Setup(_ => _.GetFor<simple_object_with_primitives>()).Returns(simple_object_with_primitives.message_description);
    };

    Because of = () =>
    {
        serialized_bytes = serializer.ToProtobuf(original);
        deserialized = serializer.FromProtobuf<simple_object_with_primitives>(serialized_bytes);
    };

    It should_hold_the_correct_guid = () => deserialized.a_guid.ShouldEqual(original.a_guid);
    It should_hold_the_correct_integer = () => deserialized.an_integer.ShouldEqual(original.an_integer);
    It should_hold_the_correct_float = () => deserialized.a_float.ShouldEqual(original.a_float);
    It should_hold_the_correct_double = () => deserialized.a_double.ShouldEqual(original.a_double);
    It should_hold_the_correct_string = () => deserialized.a_string.ShouldEqual(original.a_string);
    It should_hold_the_correct_date_time = () => deserialized.a_date_time.LossyEquals(original.a_date_time).ShouldBeTrue();
    It should_hold_the_correct_date_time_offset = () => deserialized.a_date_time_offset.LossyEquals(original.a_date_time_offset).ShouldBeTrue();
}