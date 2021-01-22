// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_ValueConverters
{
    public class when_asking_if_can_convert_with_no_converters_able : given.no_value_converters
    {
        static bool result;
        Because of = () => result = value_converters.CanConvert(typeof(string));

        It should_not_be_able = () => result.ShouldBeFalse();
    }
}