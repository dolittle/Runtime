// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptFactory
{
    [Subject(typeof(ConceptFactory))]
    public class when_creating_instance_of_double_concept_with_coming_in_as_null
    {
        static DoubleConcept result;

        Because of = () => result = ConceptFactory.CreateConceptInstance(typeof(DoubleConcept), null) as DoubleConcept;

        It should_hold_zero = () => result.Value.ShouldEqual(0.0);
    }
}
