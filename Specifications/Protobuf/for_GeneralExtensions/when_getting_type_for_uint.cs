// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_GeneralExtensions
{
    public class when_getting_type_for_uint
    {
        static Types type;

        Because of = () => type = 42U.GetProtobufType();

        It should_be_unsigned_32_bit_integer = () => type.ShouldEqual(Types.UInt32);
    }
}