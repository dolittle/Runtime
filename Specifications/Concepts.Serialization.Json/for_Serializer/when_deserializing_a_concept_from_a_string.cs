// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Serialization.Json;
using Machine.Specifications;

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer
{
    [Subject(typeof(Serializer))]
    public class when_deserializing_a_concept_from_a_string : given.a_serializer
    {
        static object deserialized;
        static string serialized_version;

        Establish context = () =>
                                {
                                    serialized_version = "\"03f1d667-063b-4d15-b892-06f89818e9a8\"";
                                };

        Because of = () =>
                         {
                             deserialized = serializer.FromJson(typeof(ConceptAsGuid), serialized_version);
                         };

        It should_create_the_guid = () => (deserialized as ConceptAsGuid).Value.ToString().ShouldEqual("03f1d667-063b-4d15-b892-06f89818e9a8");
    }
}