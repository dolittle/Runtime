// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_Serializer;

public class when_getting_length : given.a_serializer
{
    static simple_object_with_primitives object_for_serialization;
    static int length;
    static byte[] serialized_bytes;

    Establish context = () =>
    {
        object_for_serialization = new simple_object_with_primitives
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

        serialized_bytes = serializer.ToProtobuf(object_for_serialization);
    };

    Because of = () => length = serializer.GetLengthOf(object_for_serialization);

    It should_have_same_length_of_bytes_as_calculated_size = () => serialized_bytes.Length.ShouldEqual(length);
}