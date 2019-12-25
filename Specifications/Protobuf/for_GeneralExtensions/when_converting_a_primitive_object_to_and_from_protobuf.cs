// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events.Relativity.Microservice;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_GeneralExtensions
{
    public class when_converting_a_primitive_object_to_and_from_protobuf
    {
        static object str;
        static Value protobuf;
        static object result;

        Establish context = () => str = "I am a string";

        Because of = () =>
        {
            protobuf = str.ToProtobuf();
            result = protobuf.ToCLR();
        };

        It protobuf_message_should_have_an_object_value = () => protobuf.KindCase.ShouldEqual(Value.KindOneofCase.ObjectValue);
        It protobuf_message_object_value_type_should_be_string = () => protobuf.ObjectValue.Type.ShouldEqual((int)Types.String);
        It should_be_equal_to_the_original = () => result.ShouldEqual(str);
    }
}