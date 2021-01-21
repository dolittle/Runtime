// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_StringExtensions
{
    [Subject(typeof(StringExtensions))]
    public class when_converting_a_string_representation_of_a_guid_to_a_guid_concept
    {
        static string guid_as_a_string;
        static ConceptAsGuid result;

        Establish context = () =>
            {
                guid_as_a_string = Guid.NewGuid().ToString();
            };

        Because of = () => result = (ConceptAsGuid)guid_as_a_string.ParseTo(typeof(ConceptAsGuid));

        It should_create_a_guid_concept = () => result.ShouldBeOfExactType<ConceptAsGuid>();
        It should_have_the_correct_value = () => result.ToString().ShouldEqual(guid_as_a_string);
    }
}