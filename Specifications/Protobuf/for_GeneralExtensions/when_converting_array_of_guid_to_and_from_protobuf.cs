// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Events.Relativity.Microservice;
using Machine.Specifications;

namespace Dolittle.Runtime.Protobuf.for_GeneralExtensions
{
    public class when_converting_array_of_guid_to_and_from_protobuf
    {
        static Guid[] guid_array;
        static ArrayValue protobuf;
        static object[] result;

        Establish context = () => guid_array = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        Because of = () =>
        {
            protobuf = guid_array.ToProtobuf();
            result = protobuf.ToCLR();
        };

        It should_be_equal_to_the_original = () =>
        {
            var result_int_array = result.Select(_ => (Guid)_).ToArray();
            result_int_array.ShouldContainOnly(guid_array);
        };
    }
}