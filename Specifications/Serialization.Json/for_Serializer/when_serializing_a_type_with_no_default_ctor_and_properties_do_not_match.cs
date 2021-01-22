// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Json.Specs.for_Serializer
{
    [Subject(typeof(Serializer))]
    public class when_serializing_a_type_with_no_default_ctor_and_properties_do_not_match : given.a_serializer
    {
        static TypeThatCannotBeCreated to_serialize;
        static string serialized_version;

        static Exception ex;

        Establish context = () =>
        {
            to_serialize = new TypeThatCannotBeCreated("foo", "bar");
            serialized_version = serializer.ToJson(to_serialize);
        };

        Because of = () => ex = Catch.Exception(() => serializer.FromJson<TypeThatCannotBeCreated>(serialized_version));

        It should_fail_to_serialize = () => ex.ShouldNotBeNull();
        It should_indicate_that_the_type_cannot_be_created = () => ex.ShouldBeOfExactType<UnableToInstantiateInstanceOfType>();
        It should_identify_the_type_that_it_failed_to_create = () => ex.Message.ShouldContain(typeof(TypeThatCannotBeCreated).FullName);
    }
}