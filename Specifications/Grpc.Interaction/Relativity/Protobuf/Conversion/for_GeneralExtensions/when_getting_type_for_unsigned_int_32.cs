
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_GeneralExtensions
{
    public class when_getting_type_for_unsigned_int_32
    {
        static Types type;

        Because of = () => type = ((UInt32)42).GetProtobufType();

        It should_be_unsigned_32_bit_integer = () => type.ShouldEqual(Types.UInt32);
    }    
}