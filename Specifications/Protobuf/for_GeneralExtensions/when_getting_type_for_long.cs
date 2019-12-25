// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_GeneralExtensions
{
    public class when_getting_type_for_long
    {
        static Types type;

        Because of = () => type = 42L.GetProtobufType();

        It should_be_64_bit_integer = () => type.ShouldEqual(Types.Int64);
    }
}