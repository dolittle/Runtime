// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptFactory
{
    [Subject(typeof(ConceptFactory))]
    public class when_creating_instance_of_decimal_concept_with_coming_in_as_null
    {
        static DecimalConcept result;

        Because of = () => result = ConceptFactory.CreateConceptInstance(typeof(DecimalConcept), null) as DecimalConcept;

        It should_hold_zero = () => result.Value.ShouldEqual(0.0m);
    }
}