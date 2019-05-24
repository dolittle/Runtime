
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_GeneralExtensions
{
    public class when_converting_an_enumerable_object_of_primitive_to_and_from_protobuf
    {
        static object enumerable_object;
        static System.Protobuf.Value protobuf;
        static object result;

        Establish context = () => enumerable_object = new int[]{1, 2, 3};

        Because of = () => 
        {
            protobuf = enumerable_object.ToProtobuf();
            result = protobuf.ToCLR();
        };
        
        It protobuf_message_should_have_a_list_value = () => protobuf.KindCase.ShouldEqual(System.Protobuf.Value.KindOneofCase.ListValue);
        It should_be_equal_to_the_original = () =>
        {
            var enumerable = CreateArrayOf(enumerable_object, _ => (int)_);
            var result_enumerable = CreateArrayOf(result, _ => (int)_);

            result_enumerable.ShouldContainOnly(enumerable);
        };

        
        static IEnumerable<TResult> CreateArrayOf<TResult>(object arrayObject, Func<object, TResult> converterFunc) => 
            ((System.Collections.IEnumerable) arrayObject)
                .Cast<object>()
                .Select(converterFunc);
    }
}