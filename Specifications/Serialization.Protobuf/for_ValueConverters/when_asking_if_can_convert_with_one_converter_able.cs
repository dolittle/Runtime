// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_ValueConverters;

public class when_asking_if_can_convert_with_one_converter_able : given.two_value_converters
{
    static bool result;

    Establish context = () => second_value_converter.Setup(_ => _.CanConvert(typeof(string))).Returns(true);

    Because of = () => result = value_converters.CanConvert(typeof(string));

    It should_be_able = () => result.ShouldBeTrue();
}