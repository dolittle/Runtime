
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_GeneralExtensions
{
    public class when_getting_type_for_int
    {
        static Types type;

        Because of = () => type = 42.GetProtobufType();

        It should_be_32_bit_integer = () => type.ShouldEqual(Types.Int32);
    }    
}