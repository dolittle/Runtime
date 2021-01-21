// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Serialization.Json;
using Machine.Specifications;

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer
{
    [Subject(typeof(Serializer))]
    public class when_deserializing_a_concept_from_a_serialized_version : given.a_serializer
    {
        static ConceptAsGuid to_serialize;
        static object deserialized;
        static string serialized_version;

        Establish context = () =>
                                {
                                    to_serialize = Guid.NewGuid();
                                    serialized_version = serializer.ToJson(to_serialize);
                                };

        Because of = () =>
                         {
                             deserialized = serializer.FromJson(typeof(ConceptAsGuid), serialized_version);
                         };

        It should_create_the_guid_ = () => (deserialized as ConceptAsGuid).ShouldEqual(to_serialize);
    }
}