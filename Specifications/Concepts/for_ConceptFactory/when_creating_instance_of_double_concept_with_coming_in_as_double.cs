// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptFactory
{
    [Subject(typeof(ConceptFactory))]
    public class when_creating_instance_of_double_concept_with_coming_in_as_double
    {
        static DoubleConcept result;

        Because of = () => result = ConceptFactory.CreateConceptInstance(typeof(DoubleConcept), 5d) as DoubleConcept;

        It should_hold_the_double = () => result.Value.ShouldEqual(5d);
    }
}