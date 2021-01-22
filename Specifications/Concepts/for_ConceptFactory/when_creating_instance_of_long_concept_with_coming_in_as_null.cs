// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ConceptFactory
{
    public class when_creating_instance_of_long_concept_with_coming_in_as_null
    {
        static LongConcept result;

        Because of = () => result = ConceptFactory.CreateConceptInstance(typeof(LongConcept), null) as LongConcept;

        It should_hold_zero = () => result.Value.ShouldEqual(0);
    }
}
