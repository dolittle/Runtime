
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using Dolittle.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_GeneralExtensions
{
    public class when_converting_array_of_primitive_to_and_from_protobuf
    {
        static int[] int_array;
        static System.Protobuf.ArrayValue protobuf;
        static object[] result;

        Establish context = () => int_array = new int[] {1, 2, 3};

        Because of = () => 
        {
            protobuf = int_array.ToProtobuf();
            result = protobuf.ToCLR();
        };

        It should_be_equal_to_the_original = () => 
        {
            var result_int_array = result.Select(_ => (int)_).ToArray();
            result_int_array.ShouldContainOnly(int_array);
        };
    }
}