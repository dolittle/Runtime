// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_ValueConverters;

public class when_getting_value_converter_with_two_converters_capable_of_conversion : given.two_value_converters
{
    static IValueConverter result;

    Establish context = () =>
    {
        first_value_converter.Setup(_ => _.CanConvert(typeof(string))).Returns(true);
        second_value_converter.Setup(_ => _.CanConvert(typeof(string))).Returns(true);
    };

    Because of = () => result = value_converters.GetConverterFor(typeof(string));

    It should_return_first_converter = () => result.ShouldEqual(first_value_converter.Object);
}