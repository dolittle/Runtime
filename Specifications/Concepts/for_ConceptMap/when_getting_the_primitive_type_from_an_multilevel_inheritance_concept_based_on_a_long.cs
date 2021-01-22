// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;
using Dolittle.Runtime.Specs.Concepts.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptMap
{
    [Subject(typeof(ConceptMap))]
    public class when_getting_the_primitive_type_from_an_multilevel_inheritance_concept_based_on_a_long
    {
        static Type result;

        Because of = () => result = ConceptMap.GetConceptValueType(typeof(MultiLevelInheritanceConcept));

        It should_get_a_long = () => result.ShouldEqual(typeof(long));
    }
}