// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Serialization.Json;
using Machine.Specifications;

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer
{
    [Subject(typeof(Serializer))]
    public class when_serializing_a_concept : given.a_serializer
    {
        static ConceptAsGuid to_serialize;
        static string serialized_version;

        Establish context = () =>
                                {
                                    to_serialize = Guid.NewGuid();
                                };

        Because of = () => serialized_version = serializer.ToJson(to_serialize);

        It should_contain_the_guid_value = () => serialized_version.ShouldEqual("\"" + to_serialize.Value.ToString() + "\"");
    }
}