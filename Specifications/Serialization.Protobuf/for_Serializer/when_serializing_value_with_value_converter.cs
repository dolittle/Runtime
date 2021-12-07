// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Serialization.Protobuf.for_Serializer;

public class when_serializing_value_with_value_converter : given.a_serializer
{
    class special_type
    {
        public string the_value { get; set; }
    }

    class type_for_serialization
    {
        public special_type value { get; set; }
    }

    static type_for_serialization original;
    static type_for_serialization deserialized;

    Establish context = () =>
    {
        const string original_value = "Fourty two";
        original = new type_for_serialization { value = new special_type { the_value = original_value } };
        value_converters.Setup(_ => _.CanConvert(typeof(special_type))).Returns(true);

        var value_converter = new Mock<IValueConverter>();
        value_converters.Setup(_ => _.GetConverterFor(typeof(special_type))).Returns(value_converter.Object);

        const string serialized_value = "Awesome Possum";
        value_converter.Setup(_ => _.ConvertTo(original.value)).Returns(serialized_value);
        value_converter.Setup(_ => _.SerializedAs(typeof(special_type))).Returns(typeof(string));
        value_converter.Setup(_ => _.ConvertFrom(typeof(special_type), serialized_value)).Returns(new special_type { the_value = original_value });

        message_descriptions.Setup(_ => _.GetFor<type_for_serialization>()).Returns(MessageDescription.DefaultFor<type_for_serialization>());
    };

    Because of = () =>
    {
        var bytes = serializer.ToProtobuf(original);
        deserialized = serializer.FromProtobuf<type_for_serialization>(bytes);
    };

    It should_hold_the_correct_value = () => deserialized.value.the_value.ShouldEqual(original.value.the_value);
}