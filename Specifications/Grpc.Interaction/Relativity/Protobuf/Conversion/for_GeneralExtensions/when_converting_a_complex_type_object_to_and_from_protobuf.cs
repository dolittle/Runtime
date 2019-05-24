
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Concepts;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events.Relativity.Protobuf.Conversion.for_GeneralExtensions.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Grpc.Interaction.Protobuf.Conversion.for_GeneralExtensions
{
    public class when_converting_a_mutable_complex_type_object_to_and_from_protobuf
    {
        readonly static a_complex_type complex_type = new a_complex_type
        {
            Int = 1,
            Guid = Guid.NewGuid(),
            IntArray = new int[] {1, 2, 3},
            IntList = new List<int> {1, 2, 3}

        };

        static object complex_type_property_bag_object;
        static System.Protobuf.Value protobuf;
        static object result;

        Establish context = () => complex_type_property_bag_object = complex_type.ToPropertyBag();

        Because of = () => 
        {
            protobuf = complex_type_property_bag_object.ToProtobuf();
            result = protobuf.ToCLR();
        };
        
        It protobuf_message_should_have_a_dictionary_value = () => protobuf.KindCase.ShouldEqual(System.Protobuf.Value.KindOneofCase.DictionaryValue);
        It should_be_equal_to_the_original = () =>
        {
            var complex_type_property_bag = complex_type_property_bag_object as Dolittle.PropertyBags.PropertyBag;
            var result_property_bag = result as Dolittle.PropertyBags.PropertyBag;

            complex_type_property_bag["Int"].ShouldEqual(result_property_bag["Int"]);
            complex_type_property_bag["Guid"].ShouldEqual(result_property_bag["Guid"]);
            CreateArrayOf<int>(complex_type_property_bag["IntArray"], _ => (int)_).ToArray().ShouldContainOnly(CreateArrayOf(result_property_bag["IntArray"], _ => (int)_).ToArray());
            CreateArrayOf<int>(complex_type_property_bag["IntList"], _ => (int)_).ShouldContainOnly(CreateArrayOf<int>(result_property_bag["IntList"], _ => (int)_));
        };

        static IEnumerable<TResult> CreateArrayOf<TResult>(object arrayObject, Func<object, TResult> converterFunc) => 
            ((System.Collections.IEnumerable) arrayObject)
                .Cast<object>()
                .Select(converterFunc);
        
    }
}