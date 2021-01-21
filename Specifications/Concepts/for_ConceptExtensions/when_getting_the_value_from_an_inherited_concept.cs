// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Dolittle.Runtime.Specs.Concepts.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptExtensions
{
    [Subject(typeof(ConceptExtensions))]
    public class when_getting_the_value_from_an_inherited_concept : concepts
    {
        static InheritingFromLongConcept value;
        static long primitive_value = 100;
        static object returned_value;

        Establish context = () => value = new InheritingFromLongConcept { Value = primitive_value };

        Because of = () => returned_value = value.GetConceptValue();

        It should_get_the_value_of_the_primitive = () => returned_value.ShouldEqual(primitive_value);
    }
}