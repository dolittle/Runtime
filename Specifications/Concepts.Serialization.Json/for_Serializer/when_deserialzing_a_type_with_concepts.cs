// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Serialization.Json;
using Machine.Specifications;

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer
{
    [Subject(typeof(Serializer))]
    public class when_deserialzing_a_type_with_concepts : given.a_serializer
    {
        static ClassWithConcepts to_serialize;
        static string serialized_version;
        static ClassWithConcepts deserialized_version;

        Establish context = () =>
                                {
                                    to_serialize = new ClassWithConcepts()
                                                       {
                                                           GuidConcept = Guid.NewGuid(),
                                                           StringConcept = "BlahBlahBlah",
                                                           LongConcept = long.MaxValue
                                                       };

                                    serialized_version = serializer.ToJson(to_serialize);
                                };

        Because of = () => deserialized_version = serializer.FromJson<ClassWithConcepts>(serialized_version);

        It should_contain_the_guid_concept = () => deserialized_version.GuidConcept.Value.ShouldEqual(to_serialize.GuidConcept.Value);
        It should_contain_the_long_concept = () => deserialized_version.LongConcept.Value.ShouldEqual(to_serialize.LongConcept.Value);
        It should_contain_the_string_concept = () => deserialized_version.StringConcept.Value.ShouldEqual(to_serialize.StringConcept.Value);
    }
}