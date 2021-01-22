// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Serialization.Json;
using Machine.Specifications;

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer
{
    [Subject(typeof(Serializer))]
    public class when_serialzing_a_type_with_concepts : given.a_serializer
    {
        static ClassWithConcepts to_serialize;
        static string serialized_version;

        Establish context = () =>
                                {
                                    to_serialize = new ClassWithConcepts()
                                    {
                                        GuidConcept = Guid.NewGuid(),
                                        StringConcept = "BlahBlahBlah",
                                        LongConcept = long.MaxValue
                                    };
                                };

        Because of = () => serialized_version = serializer.ToJson(to_serialize);

        It should_contain_the_guid_value = () => serialized_version.IndexOf(to_serialize.GuidConcept.Value.ToString()).ShouldBeGreaterThan(0);
        It should_contain_the_long_value = () => serialized_version.IndexOf(to_serialize.LongConcept.Value.ToString()).ShouldBeGreaterThan(0);
        It should_contain_the_string_value = () => serialized_version.IndexOf(to_serialize.StringConcept.Value).ShouldBeGreaterThan(0);
    }
}