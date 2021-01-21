// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Serialization.Json;
using Machine.Specifications;

namespace Dolittle.Runtime.Concepts.Serialization.Json.Specs.for_Serializer
{
    [Subject(typeof(Serializer))]
    public class when_deserializing_a_type_inheriting_from_a_read_only_collection_with_a_constructor_taking_a_dictionary : given.a_serializer
    {
        const string key = "something";
        const string value = "fourty two";

        static string json_representation = $"{{\"{key}\":\"{value}\"}}";

        static ClassInheritingFromAReadOnlyDictionary result;

        Because of = () => result = serializer.FromJson<ClassInheritingFromAReadOnlyDictionary>(json_representation);

        It should_hold_the_key_and_value = () => result[key].ShouldEqual(value);
    }
}